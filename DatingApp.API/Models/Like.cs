namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; }
        public int LikeeId { get; set; }


        // e.g : a user (likee) could like many users 
        // and could be liked by many other users (likers)
        public User Likee { get; set; }
        public User Liker { get; set; }
        
    }
}