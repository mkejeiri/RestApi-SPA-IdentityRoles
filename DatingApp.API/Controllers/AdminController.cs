using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IManageRoleRepository _iManageRoleRepo;
        private readonly UserManager<User> _userManager;
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
         private readonly Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        public AdminController(IManageRoleRepository iManageRoleRepo, 
        UserManager<User> userManager, IDatingRepository iDatingRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userManager = userManager;
            _repo = iDatingRepo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _iManageRoleRepo = iManageRoleRepo;

              Account acc = new Account(
                    cloudinaryConfig.Value.CouldName,
                    cloudinaryConfig.Value.ApiKey,
                    cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUserWithRole()
        {
            return Ok(await _iManageRoleRepo.GetUserWithRoles());
        }


        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDTO roleEditDTO)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedUserRoles = roleEditDTO.RoleNames.ToList();


            //we assume that a user could be remove from all roles: even no longer a member!!!
            //selectedUserRoles = selectedUserRoles !=null ? selectedUserRoles : new string[] {};
            selectedUserRoles = selectedUserRoles ?? new List<string> {};

            var result = await _userManager.AddToRolesAsync(user, selectedUserRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add new roles");

            result = await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedUserRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove older roles");

            var userRolesToReturn = await _userManager.GetRolesAsync(user);
            return Ok(userRolesToReturn);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await  _iManageRoleRepo.GetPendingApprovalPhotos();           
            var photosToReturn = _mapper.Map<List<PhotoForReturnDto>>(photos);
            return Ok(photosToReturn);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhotoForModeration(int photoId)
        {
            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("You can't delete the main photo");
            } 

             if (photoFromRepo.IsMain)
            {
                return BadRequest("You cannot delete your main photo!");
            }

            if (photoFromRepo.PublicId != null)
            {
                var result = _cloudinary.Destroy(new DeletionParams(photoFromRepo.PublicId)
                {
                    ResourceType = CloudinaryDotNet.Actions.ResourceType.Image
                });

                if (result.Result == "ok")
                {
                    _repo.Delete<Photo>(photoFromRepo);
                }
            }
            else
            {
                _repo.Delete<Photo>(photoFromRepo);
            }

            if (await _repo.SaveAll()) return Ok();
            return BadRequest("Failed to delete the photo!");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhotoForModeration(int photoId)
        {

            var photoFromRepo = await _repo.GetPhoto(photoId);
            photoFromRepo.IsApproved = true;
            if (await _repo.SaveAll()) return Ok();
             return BadRequest("Failed to approve the photo!");
        }
    }
}