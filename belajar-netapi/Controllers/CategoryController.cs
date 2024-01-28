using AutoMapper;
using belajarnetapi.Dto;
using belajarnetapi.Interfaces;
using belajarnetapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace belajarnetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository category, IMapper mapper)
        {
            _categoryRepository = category;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(404)]
        public IActionResult GetCategoryById(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound();
            }
            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategoryById(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(404)]
        public IActionResult GetPokemonsByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonsByCategoryId(categoryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto category)
        {
            if (category == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.GetCategories().Any(c => c.Name.Trim().ToUpper() == category.Name.Trim().ToUpper()))
            {
                ModelState.AddModelError("", "Category Already Exists!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(category);
            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving the category {category.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Success Create Category");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDto category)
        {
            if (category == null || id != category.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryMap = _mapper.Map<Category>(category);
            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating the category {category.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Success Update Category");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategoryById(id);
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the category {category.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok("Success Delete Category");
        }
    }
}