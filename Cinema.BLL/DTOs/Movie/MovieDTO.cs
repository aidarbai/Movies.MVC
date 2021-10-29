using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Movie
{
    public class MovieDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Poster_small_path { get; set; }
        public DateTime CreationDate { get; set; }
        public string Homepage { get; set; }
        public string Backdrop_path { get; set; }
        public string Horizontal_small_image_path { get; set; }
        public DateTime? Release_date { get; set; }
        public double Vote_average { get; set; }
        public int Vote_count { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        [DisplayName("Youtube trailer")]
        public string Youtube_path { get; set; }
        public List<GenreDTO> Genres { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public List<VoteDTO> Votes { get; set; }
    }
}
