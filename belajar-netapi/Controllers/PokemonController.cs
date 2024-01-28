using System.Collections;
using AutoMapper;
using belajarnetapi.Dto;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;
using belajarnetapi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace belajarnetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, IOwnerRepository ownerRepository, ICategoryRepository categoryRepository)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            ApiResponse<ICollection<Pokemon>> response = new ApiResponse<ICollection<Pokemon>>();
            try
            {
                response.Data = _pokemonRepository.GetPokemons();
                response.Success = true;
                response.ErrorMessage = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(404)]
        public IActionResult GetPokemonById(int id)
        {
            if (!_pokemonRepository.PokemonExists(id))
            {
                return NotFound();
            }
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemonById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("rating/{id}")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(404)]
        public IActionResult GetPokemonRating(int id)
        {
            if (!_pokemonRepository.PokemonExists(id))
            {
                return NotFound();
            }
            var rating = _pokemonRepository.GetPokemonRating(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Pokemon))]
        [ProducesResponseType(404)]
        public IActionResult CreatePokemon([FromBody] PokemonDto pokemonToCreate, [FromQuery]int ownerId, [FromQuery]int categoryId)
        {
            if (pokemonToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (_pokemonRepository.GetPokemons().Any(c => c.Name.Trim().ToUpper() == pokemonToCreate.Name.Trim().ToUpper())){
                ModelState.AddModelError("", $"Pokemon {pokemonToCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            var pokemon = _mapper.Map<Pokemon>(pokemonToCreate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemon))
            {
                ModelState.AddModelError("", $"Something went wrong saving the pokemon " +
                                                $"{pokemonToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon Created");
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdatePokemon([FromQuery] int id, [FromBody] PokemonDto pokemonToUpdate, [FromQuery]int ownerId, [FromQuery]int categoryId)
        {
            if (pokemonToUpdate == null || id != pokemonToUpdate.Id || ownerId == 0 || categoryId == 0)
            {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.PokemonExists(id))
            {
                return NotFound();
            }

            var owner = _ownerRepository.GetOwnerById(ownerId);
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (owner == null || category == null)
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<Pokemon>(pokemonToUpdate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.UpdatePokemon(pokemon, owner, category))
            {
                ModelState.AddModelError("", $"Something went wrong updating the pokemon " +
                                                $"{pokemonToUpdate.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon Updated");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeletePokemon(int id)
        {
            if (!_pokemonRepository.PokemonExists(id))
            {
                return NotFound();
            }

            var pokemon = _pokemonRepository.GetPokemonById(id);
            if (!_pokemonRepository.DeletePokemon(pokemon))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the pokemon " +
                                                $"{pokemon.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon Deleted");
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            ApiResponse<ICollection<PokemonOwner>> response = new ApiResponse<ICollection<PokemonOwner>>();
            try
            {
                response.Data = _pokemonRepository.GetPokemonsAndOwner();
                response.Success = true;
                response.ErrorMessage = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}