using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BestMovies.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
