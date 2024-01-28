using belajarnetapi.Models;

namespace belajarnetapi.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        ICollection<PokemonOwner> GetPokemonsAndOwner();
        Pokemon GetPokemonById(int id);
        Pokemon GetPokemonByName(string name);
        decimal GetPokemonRating(int id);
        bool PokemonExists(int id);
        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool UpdatePokemon(Pokemon pokemon, Owner owner, Category category);
        bool DeletePokemon(Pokemon pokemon);
        bool Save();
    }
}