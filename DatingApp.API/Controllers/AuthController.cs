using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// using System.Collections.Generic;
// using System.Linq;

// using DatingApp.API.Data;
// using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    //[ApiController]
    /* ApiController simplified a lot of stuff such as ->
        if we remove API controller we no longer get:
            -  string values such username and password filled-in as string empty but as null,
             and operation on string crashes, we could solve this PB by adding:
                Register ([FromBody]UserForRegisterDto userForRegisterDto) 
            - but in doing so we need also to call ModelState.valid() to be able to check the annotation 
            (eg: required, stringlength) and send a bad request our self: e.g if(!ModelState.IsValid) return BadRequest(ModelState);
    */
    public class AuthController : ControllerBase
    {       
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration config, IMapper mapper,
                             UserManager<User> userManager,
                             SignInManager<User> signInManager)
        {
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //we don't need to use [FromBody], because dotnet inferer auto from the post request body 
        //used by userForRegisterDto
        // public async Task<IActionResult> Register ([FromBody]UserForRegisterDto userForRegisterDto){
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.password);
            
            if (result.Succeeded)            
            {
                var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
                return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {

            var user = await _userManager.FindByNameAsync(userForLoginDto.username);
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.username.ToUpper());
                var userToReturn = _mapper.Map<UserForListDto>(appUser);
                return Ok(new
                {
                    token = GetToken(user),
                    user = userToReturn
                });
            }
            return Unauthorized();
        }

        private string GetToken(User user)
        {
            //Start Token building
            //1- create a claims so that the server doesn't go to the DB to check for credentials: Id & username      
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
            };

            /*
                Token server signing!: to make sure that when the token is comming back is valid!
            */
            //2-Key for the token which will be hashed!, encoded to a bytearray
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //3- signing the key AppSetting:Token with hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //4-create a token descriptor, based on the claims, expiration day and hashed key
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            //5- JwtSecurityTokenHandler allows us to create a token based on the token passed above 
            var tokenHandler = new JwtSecurityTokenHandler();
            //6- store the token in var token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}