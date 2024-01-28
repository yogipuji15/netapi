using AutoMapper;
using belajarnetapi.Dto;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace belajarnetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewerById(int id)
        {
            if (!_reviewerRepository.ReviewerExists(id))
            {
                return NotFound();
            }
            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewerById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewer);
        }

        [HttpGet("review/{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsOfAReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            var reviewer = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewer);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(404)]
        public IActionResult CreateReviewer([FromBody]ReviewerDto reviewerToCreate)
        {
            if (reviewerToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (_reviewerRepository.GetReviewers().Any(r => r.FirstName.Trim().ToUpper() == reviewerToCreate.FirstName.Trim().ToUpper() &&
                                                            r.LastName.Trim().ToUpper() == reviewerToCreate.LastName.Trim().ToUpper()))
            {
                ModelState.AddModelError("", $"Reviewer {reviewerToCreate.FirstName} {reviewerToCreate.LastName} already exists");
                return StatusCode(422, ModelState);
            }

            var reviewer = _mapper.Map<Reviewer>(reviewerToCreate);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.CreateReviewer(reviewer))
            {
                ModelState.AddModelError("", $"Something went wrong saving the reviewer " +
                                                $"{reviewerToCreate.FirstName} {reviewerToCreate.LastName}");
                return StatusCode(500, ModelState);
            }
            return Ok("Reviewer Created!");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int id, [FromBody]ReviewerDto reviewerToUpdate)
        {
            if (reviewerToUpdate == null || id != reviewerToUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist!");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            var reviewer = _mapper.Map<Reviewer>(reviewerToUpdate);
            if (!_reviewerRepository.UpdateReviewer(reviewer))
            {
                ModelState.AddModelError("", $"Something went wrong updating the reviewer " +
                                                $"{reviewerToUpdate.FirstName} {reviewerToUpdate.LastName}");
                return StatusCode(500, ModelState);
            }
            return Ok("Reviewer Updated!");
        }
    }
}