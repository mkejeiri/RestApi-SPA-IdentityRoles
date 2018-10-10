using System.Collections.Generic;

namespace DatingApp.API.Helpers
{
    public class UserWithRole
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string[] Roles { get; set; }
    }
}