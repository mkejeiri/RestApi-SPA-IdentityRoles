using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using DatingApp_IdentityRoles.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedUser()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                //Hardcode roles to put user into
                var roles = new List<Role>() {
                    new Role() { Name ="Member"},
                    new Role() { Name ="Admin"},
                    new Role() { Name ="Moderator"},
                    new Role() { Name ="VIP"}
                }; 

                //create some role in the table 
                foreach (var role in roles)_roleManager.CreateAsync(role).Wait();
                
                //create user in the table 
                foreach (var user in users) {
                    _userManager.CreateAsync(user, "password").Wait();
                    _userManager.AddToRoleAsync(user, "Member").Wait();
                    }

                //create an Admin user who has access to the full App
                var adminUser = new User(){
                    UserName="Admin"
                };

                //we won't create AdminUser yet, no need for Wait() operator
                //adminUser is a type of identity result
                //Store the result in the result variable
                IdentityResult result = _userManager.CreateAsync(adminUser,"password").Result;
                if (result.Succeeded)
                {
                    //.Result : get the result value of System.Threading.Task which mean 
                    //admin will be a type of user (unlike adminUser is a type of identity result!)
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin,new [] {"Admin", "Moderator"}).Wait();
                }                
            }
        }
    }
}