using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DAL.Models
{
    public class Genres
    {
        public Genres()
        {
            Movies = new HashSet<Movie>();
        }
        [JsonIgnore]
        [Key]
        public int Id { get; set; }
        [JsonProperty("id")]
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Movie> Movies { get; set; }
    }
}
