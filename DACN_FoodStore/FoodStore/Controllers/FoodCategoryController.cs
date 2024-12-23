using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodCategoryController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _foodcategoryRepository;
        private readonly IMapper _mapper;

        public FoodCategoryController(IFoodRepository foodRepository, IFoodCategoryRepository foodcategoryRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _foodcategoryRepository = foodcategoryRepository;
            _mapper = mapper;
        }

        // GET: api/FoodCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodCategory>>> GetAllCategories()
        {
            var categories = await _foodcategoryRepository.GetAllAsync();
            // map từ food sang foodDTO
            return Ok(_mapper.Map<List<FoodCategoryDTO>>(categories));
        }
        // GET: api/FoodCategory/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var foodCategory = await _foodcategoryRepository.GetByIdAsync(id);
            if (foodCategory == null)
            {
                return NotFound();
            }
            return Ok(foodCategory);
        }

        // POST: api/FoodCategory
        [HttpPost]
        public async Task<IActionResult> CreateCategory(FoodCategory foodCategory)
        {
            if (ModelState.IsValid)
            {
                await _foodcategoryRepository.AddAsync(foodCategory);
                return CreatedAtAction(nameof(GetCategoryById), new { id = foodCategory.Id }, foodCategory);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/FoodCategory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, FoodCategory foodCategory)
        {
            if (id != foodCategory.Id)
            {
                return BadRequest("ID in URL does not match ID in model.");
            }

            if (ModelState.IsValid)
            {
                await _foodcategoryRepository.UpdateAsync(foodCategory);
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/FoodCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var foodCategory = await _foodcategoryRepository.GetByIdAsync(id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            await _foodcategoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
