using System;
using System.ComponentModel.DataAnnotations;
//Data transfer object (DTO)
namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specified a password between 4 and 8 characters")]
        public string password { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public string knownAs { get; set; }
        [Required]
        public DateTime dateOfBirth { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string country { get; set; }
        public DateTime created { get; set; }
        public DateTime lastActive { get; set; }
        public UserForRegisterDto()

        {
            created = DateTime.Now;
            lastActive = DateTime.Now;
        }
    }
}
