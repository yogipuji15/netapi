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
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(404)]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewById(int id)
        {
            if (!_reviewRepository.ReviewExists(id))
            {
                return NotFound();
            }
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReviewById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(review);
        }

        [HttpGet("pokemon/{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsByPokemon(int pokemonId)
        {
            if (!_reviewRepository.ReviewExists(pokemonId))
            {
                return NotFound();
            }
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsByPokemon(pokemonId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(404)]
        public IActionResult CreateReview([FromBody] ReviewDto reviewToCreate, [FromQuery] int pokemonId, [FromQuery] int reviewerId)
        {
            if (reviewToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (_reviewRepository.GetReviews().Any(c => c.Title.Trim().ToUpper() == reviewToCreate.Title.Trim().ToUpper()))
            {
                ModelState.AddModelError("", "Pokemon doesn't exist!");
                return StatusCode(404, ModelState);
            }

            var pokemon = _pokemonRepository.GetPokemonById(pokemonId);
            if (pokemon == null)
            {
                ModelState.AddModelError("", "Pokemon doesn't exist!");
                return StatusCode(404, ModelState);
            }

            var reviewer = _reviewerRepository.GetReviewerById(reviewerId);
            if (reviewer == null)
            {
                ModelState.AddModelError("", "Reviewer doesn't exist!");
                return StatusCode(404, ModelState);
            }

            var review = _mapper.Map<Review>(reviewToCreate);
            review.Pokemon = pokemon;
            review.Reviewer = reviewer;
            if (!_reviewRepository.CreateReview(review))
            {
                ModelState.AddModelError("", $"Something went wrong saving the review");
                return StatusCode(500, ModelState);
            }
            
            return Ok("Review Created!");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int id, [FromBody] ReviewDto reviewToUpdate)
        {
            if (reviewToUpdate == null || id != reviewToUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.ReviewExists(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = _mapper.Map<Review>(reviewToUpdate);
            if (!_reviewRepository.UpdateReview(review))
            {
                ModelState.AddModelError("", $"Something went wrong updating the review");
                return StatusCode(500, ModelState);
            }

            return Ok("Review Updated!");
        }
    }
}