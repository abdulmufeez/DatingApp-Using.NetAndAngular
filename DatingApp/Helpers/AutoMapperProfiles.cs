using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;

namespace DatingApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Telling mapper to map to sourcae to destination object
            // <Source => Destination>
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<UserProfile, UserProfileDto>()
                // Configuring automapper so that it pick property from child class and assign it to parent class
                .ForMember(destination => destination.MainPhotoUrl, 
                options => options.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.ApplicationUser.UserName));            
            CreateMap<UserProfileCreateDto, UserProfile>();
            CreateMap<UserProfileUpdateDto, UserProfile>();
            CreateMap<Photo, PhotoDto>();            
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
            // converting local datetime to utcdatetime and send it to client
            CreateMap<DateTime, DateTime>().ConvertUsing(date => DateTime.SpecifyKind(date, DateTimeKind.Utc));
        }
    }
}