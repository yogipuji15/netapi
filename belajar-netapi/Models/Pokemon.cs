using System.ComponentModel.DataAnnotations;

namespace belajarnetapi.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<PokemonCategory> PokemonCategories { get; set; }
        public ICollection<PokemonOwner> PokemonOwners { get; set; }
    }
}