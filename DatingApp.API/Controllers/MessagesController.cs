using System.Security.Claims;
using AutoMapper;
using DatingApp.API.Data;
using System.Linq;
using DatingApp.API.Helpers;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{   
    //http://localhost:5000/api/users/userId/controller]
    // [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))] //executed after  to update lastActive
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int messageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId,userId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            
            var messageFromRepo = await _repo.GetMessage(messageId);

            // if (messageFromRepo == null)
            // {
            //     return NotFound();
            // }
            // if (messageFromRepo.SenderId != userId || messageFromRepo.RecipientId != userId)
            // {
            //     return Unauthorized();
            // }
            return Ok(messageFromRepo);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,
                MessageForCreationDto messageForCreationDto)                
        {
            var sender = await  _repo.GetUser(userId,false);
            // if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            // if (messageForCreationDto.RecipientId == userId)
            // {
            //     return BadRequest("You cannot send a message to your self");
            // }
            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId,false);

            if (recipient == null)
            {
                return BadRequest("Couldn't find the member");
            }

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);
            
            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
            /*
                The CreatedAtRoute method is intended to return a URI 
                to the newly created resource when you invoke a POST 
                method to store some new object. 
                So if you POST an order item for instance, 
                you might return a route like 'api/order/11' 
                (11 being the id of the order obviously)
            */

                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            }
            else
            {
                throw new Exception("Couldn't send message");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            messageParams.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage,
              messagesFromRepo.PageSize, messagesFromRepo.TotalCount,
               messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var messagesFromRepo = await _repo.GetMessagesThread(userId,recipientId);
            
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            return Ok(messageThread);
        }


         [HttpPost("{id}") ]
        public async Task<IActionResult> GetMessageForUser(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo.SenderId == userId) messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientId == userId) messageFromRepo.RecipientDeleted = true;
            if (messageFromRepo.SenderDeleted && 
                messageFromRepo.RecipientDeleted) _repo.Delete(messageFromRepo);
            if (await _repo.SaveAll()) return NoContent();
            throw new Exception("Error deleting messages");
        }

        [HttpPost("{id}/read") ]
         public async Task<IActionResult>MarkAsRead(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var message = await _repo.GetMessage(id);
            if (message.RecipientId != userId) return Unauthorized();
            message.IsRead=true;
            message.DateRead = DateTime.Now;
            if (await _repo.SaveAll()) return NoContent();
            throw new Exception("Error when marking message as read");
        }
    }
}