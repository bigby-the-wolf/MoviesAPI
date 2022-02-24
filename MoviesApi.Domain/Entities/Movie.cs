using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Domain.Entities
{
    public class Movie
    {
        public Movie(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
