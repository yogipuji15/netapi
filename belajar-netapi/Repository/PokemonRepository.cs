using belajarnetapi.Data;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;
using Microsoft.EntityFrameworkCore;

namespace belajarnetapi.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p=>p.Id).ToList();
        }

        public ICollection<PokemonOwner> GetPokemonsAndOwner()
        {
            try
            {
                return _context.PokemonOwners.Include(p=>p.Owner).Select(po=>new PokemonOwner(){
                    Pokemon = po.Pokemon,
                    Owner = po.Owner
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An internal server error occurred.");
            }
        }

        public Pokemon GetPokemonById(int id)
        {
            try
            {
                return _context.Pokemons.Where(p=>p.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("An internal server error occurred.");
            }
        }

        public Pokemon GetPokemonByName(string name)
        {
            return _context.Pokemons.Where(p=>p.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
        }

        public decimal GetPokemonRating(int id)
        {
            var reviews = _context.Reviews.Where(r=>r.Pokemon.Id == id).ToList();
            decimal total = 0;
            total += (decimal)reviews.Sum(r=>r.Rating)/reviews.Count();
            return total;
        }

        public bool PokemonExists(int id)
        {
            return _context.Pokemons.Any(p=>p.Id == id);
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var owner = _context.Owners.Where(o=>o.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(c=>c.Id == categoryId).FirstOrDefault();
            if(owner == null || category == null)
            {
                return false;
            }

            var pokemonOwner = new PokemonOwner(){
                Owner=owner,
                Pokemon = pokemon
            };
            
            var pokemonCategory = new PokemonCategory(){
                Category = category,
                Pokemon = pokemon
            };
            
            _context.PokemonOwners.Add(pokemonOwner);
            _context.PokemonCategories.Add(pokemonCategory);
            _context.Pokemons.Add(pokemon);
            return Save();
        }

        public bool UpdatePokemon(Pokemon pokemon, Owner owner, Category category)
        {
            var pokemonOwner = _context.PokemonOwners.Where(po=>po.Pokemon.Id == pokemon.Id).FirstOrDefault();
            var pokemonCategory = _context.PokemonCategories.Where(pc=>pc.Pokemon.Id == pokemon.Id).FirstOrDefault();
            if(pokemonOwner == null || pokemonCategory == null)
            {
                return false;
            }

            _context.Remove(pokemonOwner);
            _context.Remove(pokemonCategory);

            if(Save() == false)
            {
                return false;
            }
            
            pokemonOwner.OwnerId = owner.Id;
            pokemonCategory.CategoryId = category.Id;
            pokemonOwner.Owner = owner;
            pokemonCategory.Category = category;
            
            _context.Add(pokemonOwner);
            _context.Add(pokemonCategory);
            _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved >= 0 ? true : false;
        }
    }
}