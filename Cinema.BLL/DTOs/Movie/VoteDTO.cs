using Cinema.BLL.DTOs.User;

namespace Cinema.BLL.DTOs.Movie
{
    public class VoteDTO
    {
        public int Id { get; set; }
        public bool IsUpVote { get; set; }
        public int MovieId { get; set; }
        public ApplicationUserDTO User { get; set; }
    }
}
