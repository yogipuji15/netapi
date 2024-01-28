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
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository , IMapper mapper, IMapper mapper1)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
            _countryRepository = countryRepository;
        }
        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(404)]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(404)]
        public IActionResult GetOwnerById(int id)
        {
            if (!_ownerRepository.OwnerExists(id))
            {
                return NotFound();
            }
            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwnerById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owner);
        }

        [HttpGet("pokemon/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(404)]
        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonsByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        // [HttpGet("pokemon/{pokemonId}")]
        // [ProducesResponseType(200, Type = typeof(Owner))]
        // [ProducesResponseType(404)]
        // public IActionResult GetOwnerByPokemonId(int pokemonId)
        // {
        //     if (!_ownerRepository.OwnerExists(pokemonId))
        //     {
        //         return NotFound();
        //     }
        //     var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwnerByPokemonId(pokemonId));
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     return Ok(owner);
        // }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromBody] OwnerDto owner, [FromQuery] int countryId)
        {
            if (owner == null)
            {
                return BadRequest(ModelState);
            }
            if (_ownerRepository.OwnerExists(owner.Id))
            {
                ModelState.AddModelError("", "Owner Exists!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerObj = _mapper.Map<Owner>(owner);
            ownerObj.Country = _countryRepository.GetCountryById(countryId);
            if (!_ownerRepository.CreateOwner(ownerObj))
            {
                ModelState.AddModelError("", $"Something went wrong saving the owner {owner.FirstName} {owner.LastName}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetOwnerById", new { id = ownerObj.Id }, ownerObj);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int id, [FromBody] OwnerDto owner)
        {
            if (owner == null || id != owner.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_ownerRepository.OwnerExists(id))
            {
                ModelState.AddModelError("", "Owner Not Found!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerObj = _mapper.Map<Owner>(owner);
            if (!_ownerRepository.UpdateOwner(ownerObj))
            {
                ModelState.AddModelError("", $"Something went wrong updating the owner {owner.FirstName} {owner.LastName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}