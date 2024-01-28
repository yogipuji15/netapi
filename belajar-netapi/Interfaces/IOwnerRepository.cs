using belajarnetapi.Models;

namespace belajarnetapi.Repository
{
    public interface IOwnerRepository
    {
        public ICollection<Owner> GetOwners();
        public Owner GetOwnerById(int id);
        public Owner GetOwnerByPokemonId(int pokemonId);
        public ICollection<Pokemon> GetPokemonsByOwner(int ownerId);
        public bool OwnerExists(int id);
        public bool CreateOwner(Owner owner);
        public bool UpdateOwner(Owner owner);
        public bool Save();
    }
}