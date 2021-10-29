using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Cinema.DAL.Models
{
    public class Movie
    {
        public Movie()
        {
            Genres = new HashSet<Genres>();
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }
        [JsonIgnore]
        [Key]
        public int Id { get; set; }
        [JsonProperty("id")]
        public int ExternalId { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string Poster_path { get; set; }
        public string Poster_small_path { get; set; }
        public string Backdrop_path { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Homepage { get; set; }
        public string Horizontal_small_image_path { get; set; }
        public DateTime? Release_date { get; set; }
        public double Vote_average { get; set; }
        public int Vote_count { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public string Youtube_path { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Genres> Genres { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}
