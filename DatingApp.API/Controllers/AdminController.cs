using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IManageRoleRepository _repo;
        private readonly UserManager<User> _userManager;

        public AdminController(IManageRoleRepository repo, UserManager<User> userManager)
        {
            _userManager = userManager;
            _repo = repo;
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUserWithRole()
        {
            return Ok(await _repo.GetUserWithRoles());
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
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }

        [Authorize(Policy = "VipOnly")]
        [HttpGet("vip")]
        public IActionResult GetVip()
        {
            return Ok("Only VIP can see this");
        }
    }
}