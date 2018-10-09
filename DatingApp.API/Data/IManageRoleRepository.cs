using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IManageRoleRepository
    {
         Task<List<UserWithRole>> GetUserWithRoles();
    }
}