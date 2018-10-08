
//Data transfer object (DTO)
using System;

namespace DatingApp.API.Dtos
{
    public class MessageForCreationDto
    {
        // public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime? MessageSent { get; set; }   

        public MessageForCreationDto()        
        {
            MessageSent = DateTime.Now;
        }
    }
}