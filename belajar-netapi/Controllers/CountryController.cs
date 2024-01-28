using AutoMapper;
using belajarnetapi.Dto;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace belajarnetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(404)]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(countries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(404)]
        public IActionResult GetCountryById(int id)
        {
            if (!_countryRepository.CountryExists(id))
            {
                return NotFound();
            }
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(404)]
        public IActionResult GetCountryByOwnerId(int ownerId)
        {
            if (!_countryRepository.CountryExists(ownerId))
            {
                return NotFound();
            }
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto country)
        {
            if (country == null)
            {
                return BadRequest(ModelState);
            }
            if (_countryRepository.GetCountries().Any(c => c.Name.Trim().ToUpper() == country.Name.Trim().ToUpper()))
            {
                ModelState.AddModelError("", "Country Already Exists!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryMap = _mapper.Map<Country>(country);
            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the country {country.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Success Create Country");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int id, [FromBody] CountryDto country)
        {
            if (country == null || id != country.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExists(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryMap = _mapper.Map<Country>(country);
            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the country {country.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Success Update Country");
        }
    }
}