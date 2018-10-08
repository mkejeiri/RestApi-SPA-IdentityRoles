using System;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        /*      
        I M P O R T A N T 
        it's important to keep the properties name ending with KnownAs & PhotoUrl
        because automapper is clever enough to figure out that those are comming 
        from navigation property of the user. because the Sender/Recipient is type of USER which are 
        linked through SenderId = User.Id and RecipientId = User.Id, so removing those prefixes from
        the properties will result exaclty on property names of the user (e.g. KnownAs & PhotoUrl).
        */
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }        
        public string RecipientKnownAs { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }

        // We want a null in case of the user didn't read his email!
        // not a default value from a 100 years ago.
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}