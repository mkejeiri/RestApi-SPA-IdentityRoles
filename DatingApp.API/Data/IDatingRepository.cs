using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetUserLike(int userId, int recipientId);

        //we pass the location of this message CreatedAtRoute!
        Task<Message> GetMessage(int id);
        //used for inbox and outbox 
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

        //retrieve conversation between two users in tab panel
        Task<IEnumerable<Message>> GetMessagesThread(int userId, int messageId);
    }
}