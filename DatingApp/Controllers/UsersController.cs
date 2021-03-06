using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Helpers;

namespace DatingApp.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {        
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork, 
            IMapper mapper, IPhotoService photoService)
        {     
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsers([FromQuery]UserProfileParams userProfileParams)
        {
            var currentUserProfileId = User.GetAppUserId();            
            var gender = await _unitOfWork.UserProfileRepository.GetUserGender(currentUserProfileId);
            
            userProfileParams.CurrentUserId = currentUserProfileId;
            
            if (string.IsNullOrEmpty(userProfileParams.Gender))
                userProfileParams.Gender = gender == "male" ? "female" : "male";    

            var userProfiles = await _unitOfWork.UserProfileRepository.GetUserProfilesAsync(userProfileParams);
            Response.AddPaginationHeader(userProfiles.CurrentPage, userProfiles.PageSize, 
                userProfiles.TotalCount, userProfiles.TotalPages);

            return Ok(userProfiles);
        }

        //api/users/2
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<UserProfileDto>> GetUser(string username)
        {
            var currentUsername = User.GetUsername();

            return await _unitOfWork.UserProfileRepository.GetUserProfileByUsernameAsync(username,
                isCurrentUser: currentUsername == username);
        }      

        [HttpPost("add-profile")]
        public async Task<ActionResult> AddUser(UserProfileCreateDto userProfileDto)
        {            
            var userProfile = _mapper.Map<UserProfile>(userProfileDto);            
            
            _unitOfWork.UserProfileRepository.Add(userProfile);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem adding Profile");
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserProfileUpdateDto userProfileUpdateDto)
        {
            var user = await _unitOfWork.UserProfileRepository.GetUserByIdAsync(userProfileUpdateDto.Id);

            _mapper.Map(userProfileUpdateDto, user);

            _unitOfWork.UserProfileRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var userProfile = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(User.GetAppUserId());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };            

            userProfile.Photos.Add(photo);

            if (await _unitOfWork.Complete())
            {
                // this will include the location in headers where you get the photos
                return CreatedAtRoute("GetUser", new { username = userProfile.ApplicationUser.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var userProfile = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(User.GetAppUserId());

            var photo = userProfile.Photos.SingleOrDefault(p => p.Id == photoId);

            if (!photo.IsApprove) return BadRequest("This has to be approve before set to main");

            if (photo.IsMain) return BadRequest("This Photo is already a main photo");

            var currentMainPhoto = userProfile.Photos.SingleOrDefault(p => p.IsMain);

            if (currentMainPhoto != null) currentMainPhoto.IsMain = false;

            photo.IsMain = true;

            if (await _unitOfWork.Complete()) return NoContent();            

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var userProfile = await _unitOfWork.UserProfileRepository.GetUserByAppIdAsync(User.GetAppUserId());
            var photo = userProfile.Photos.SingleOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Cannot Delete Main Photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            userProfile.Photos.Remove(photo);
            
            if (await _unitOfWork.Complete()) return NoContent();            

            return BadRequest("Failed to delete the photo");
        }
    }
}