using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DatingApp.API.Dtos;
using System;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DatingApp.API.Models;
using System.Linq;

namespace DatingApp.API.Controllers
{
    // [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        public readonly IDatingRepository _repo;
        public readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                    cloudinaryConfig.Value.CouldName,
                    cloudinaryConfig.Value.ApiKey,
                    cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
         [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            /*
                 we need to match the user attempting to update his profile matching the id which part of the token in the server
               */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);
            var file = photoForCreationDto.File;

            //uploadResult is storing the result we getting back from Cloudinary
            var uploadResult = new ImageUploadResult();

            //if the file length we getting from the user is > 0 the we read it. 
            if (file.Length > 0)
            {
                //stream read the file loaded into memory!
                using (var stream = file.OpenReadStream())
                {

                    //we provide Cloudinary params as an ImageUploadParams object!
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    //uploadResult is storing the result we getting back from Cloudinary
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            //fill in the info we got from Cloudinary response : url to photo in Cloudinary + publicId
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            //map the photoForCreationDto to Photo object for persistance.
            var photo = _mapper.Map<Photo>(photoForCreationDto);

            //photo.IsMain = !userFroRepo.Photos.Any(u => u.IsMain)
            //set main photo to true if the first photo
            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);
            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

                //we return to the user the CreatedAtRoute with the location header with the location of created result.
                //we have to provide the root to get an individual photo for the current user (the photo is stored in cloudinary!!!)
                return CreatedAtRoute(/*string routename = */ "GetPhoto",
                                      /*routeValue= id of the photo*/ new { id = photo.Id },
                                      /*objectValue = entity to return*/ photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        /*
                                            I N F O:
            - This HttpPost is used after we send back a response to client/user via CreatedAtRoute("GetPhoto",new { id = photo.Id }, photoToReturn) to get
                and individual photo
            - We use this to get an individual photo (stored in cloudinary) location & publicId from our DB. 
        */
        [HttpPost(/*Id of the photo*/ "{id}",
                 /*routename*/ Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            /*
            1- Since Photo include navigation property to user, 
                i.e user data will be sent over back to client with hashed password, so 
                for security reasons we ought to use PhotoForReturnDto instead!

            2- we could use PhotoForDetailedDto instead of creating a new PhotoForReturnDto but this is used for 
                another purpose which is sending back photo details without also the nav user property.
            */
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);  

            /*
                check if a main photo exist and the current photo belong to the current user
            */
            if (!userFromRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("This is already a main photo");
            }

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;
            if (!await _repo.SaveAll()) return BadRequest("Could not set to main");
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

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
    }
}