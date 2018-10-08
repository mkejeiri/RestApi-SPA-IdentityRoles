
namespace DatingApp.API.Helpers
{
    //
        // Summary:
        //     Used for pagination params.
        public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string Gender { get; set; }
        public int UserId { get; set; }

        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;

        public string OrderBy {get ; set;}  
        public bool likers { get; set; } = false;
        public bool likees { get; set; } = false;
          
    }
}