namespace Cinema.DAL.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public bool IsUpVote { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
