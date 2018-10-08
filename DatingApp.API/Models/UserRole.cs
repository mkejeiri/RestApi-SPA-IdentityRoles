using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;

namespace DatingApp_IdentityRoles.API.Models
{
    public class UserRole: IdentityUserRole<int>
    {
        public Role Role { get; set; }
        public User User { get; set; }
    }
}