using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    //http://localhost:5000/api/[controller]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))] //executed after  to update lastActive
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userParams.UserId = currentUserId;
            var currentUser = await _repo.GetUser(currentUserId);

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender.ToLower() == "male" ? "female" : "male";
            }



            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            //add pagination header in the response to the client.
            //int currentPage, int itemsPerPage, int totalItems, int totalPages
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        // [AllowAnonymous]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {

            /*
              we need to match the user attempting to update his profile matching the id which part of the token in the server
            */
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFroRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdateDto, userFroRepo);

            if (await _repo.SaveAll()) return NoContent();

            throw new Exception($"updating user with {id} failed");
        }


        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {

            //Check if user Identity
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var like = await _repo.GetUserLike(id, recipientId);
            var recipientUser = await _repo.GetUser(recipientId);

            if (like != null)
            {
                return BadRequest($"You've already liked {recipientUser.KnownAs}");
            }

            //    if (await _repo.GetUser(recipientId) == null)
            if (recipientUser == null)
            {
                return NotFound();
            }


            //Business shouldn't be here: Demo purpose
            like = new Like()
            {
                LikeeId = recipientId,
                LikerId = id
            };

            _repo.Add(like);

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Unable to like the member");
        }


    }
}
