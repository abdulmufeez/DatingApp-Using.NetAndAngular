using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;        
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;        
        public MessageHub (IUnitOfWork unitOfWork,
            IMapper mapper,            
            IHubContext<PresenceHub> presenceHub,
            PresenceTracker presenceTracker)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;            
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var recipientUser = httpContext.Request.Query["otherusername"].ToString();

            var senderName = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(Context.User.GetAppUserId());
            var recipientName = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(int.Parse(recipientUser));

            var groupName = GetGroupName(senderName.KnownAs, recipientName.KnownAs);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToMessageGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdateGroup", group);

            var messages = await _unitOfWork.MessageRepository.GetMessagesThread(senderName.Id, recipientName.Id);

            if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

            await Clients.Caller.SendAsync("MessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdateGroup", group);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageAsync(CreateMessageDto createMessageDto)
        {                        
            var sender = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(Context.User.GetAppUserId());
            
            if (sender.Id == createMessageDto.RecipientId)
                throw new HubException("You cannot send messages to yourself");

            var recipient = await _unitOfWork.UserProfileRepository.GetUserByIdAsync(createMessageDto.RecipientId);

            if (recipient is null) throw new HubException($"{recipient.KnownAs} not found");

            var message = new Message
            {
                Sender = sender,
                SenderId = sender.Id,
                SenderName = sender.KnownAs,
                SenderUsername = sender.ApplicationUser.UserName,
                Recipient = recipient,
                RecipientId = recipient.Id,
                RecipientName = recipient.KnownAs,
                RecipientUsername = recipient.ApplicationUser.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.KnownAs, recipient.KnownAs);
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(u => u.UserProfileId == recipient.Id))
            {
                message.MessageRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _presenceTracker.GetConnectionForUser(recipient.KnownAs);
                
                if (connections is not null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("MessageReceived",
                        new { UserProfileId = sender.Id, KnownAs = sender.KnownAs });
                }
            }

            _unitOfWork.MessageRepository.AddMessage(message);
            
            if (await _unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> AddToMessageGroup(string groupName)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetAppUserId());

            if (group is null)
            {
                group = new Group(groupName);
                _unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _unitOfWork.Complete()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.MessageRepository.GetGroupFormConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            
            _unitOfWork.MessageRepository.RemoveConnection(connection);
            
            if (await _unitOfWork.Complete()) return group;

            throw new HubException("Failed to remove group");
        }

        private string GetGroupName(string caller, string other)
        {
            // compareOrdinal matches two string             
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}