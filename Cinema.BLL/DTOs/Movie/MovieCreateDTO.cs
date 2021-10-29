using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Movie
{
    public class MovieCreateDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "This field is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Overview { get; set; }

        public string Youtube_path { get; set; }
    }
}
