namespace Cinema.DAL.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
