using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }

        // We want a null in case of the user didn't read his email!
        // not a default value from a 100 years ago.
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        //track the message if deleted by both side then delete the msg from the table
        //If the message deleted by the sender
        public bool SenderDeleted { get; set; }

        //If the message deleted by the recipient
        public bool RecipientDeleted { get; set; }


    }
}