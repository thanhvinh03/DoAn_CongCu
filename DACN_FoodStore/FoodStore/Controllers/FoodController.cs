using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _foodcategoryRepository;
        private readonly IMapper _mapper;

        public FoodController(IFoodRepository foodRepository, IFoodCategoryRepository foodcategoryRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _foodcategoryRepository = foodcategoryRepository;
            _mapper = mapper;
        }

        // Get all foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
            var foods = await _foodRepository.GetAllAsync();
            // map từ food sang foodDTO
            return Ok(_mapper.Map<List<FoodDTO>>(foods));
        }

        // Get food by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FoodDTO>(food));
        }

        // Get list food by id category
        [HttpGet("category/{id}")]
        public async Task<ActionResult<Food>> GetListFoodByCategory(int id)
        {
            var food = await _foodRepository.GetListFoodByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<FoodDTO>> (food));
        }

        // Add a new food
        [HttpPost]
        public async Task<ActionResult<Food>> AddFood([FromForm] Food food, [FromForm] IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (imageUrl != null)
            {
                food.Image = await SaveImage(imageUrl);
            }

            await _foodRepository.AddAsync(food);
            return CreatedAtAction(nameof(GetFood), new { id = food.Id }, food);
        }

        // Update an existing food
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(int id, [FromForm] Food food, [FromForm] IFormFile imageUrl)
        {
            if (id != food.Id)
            {
                return BadRequest();
            }

            ModelState.Remove("ImageUrl"); // Ignore model validation for imageUrl

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFood = await _foodRepository.GetByIdAsync(id);
            if (existingFood == null)
            {
                return NotFound();
            }

            // Update image if a new one is provided
            if (imageUrl == null)
            {
                food.Image = existingFood.Image;
            }
            else
            {
                food.Image = await SaveImage(imageUrl);
            }

            // Update food details
            existingFood.Name = food.Name;
            existingFood.Price = food.Price;
            existingFood.FoodCategoryId = food.FoodCategoryId;
            existingFood.Image = food.Image;
            existingFood.Status = food.Status;
            existingFood.Type = food.Type;

            await _foodRepository.UpdateAsync(existingFood);

            return NoContent(); // Indicating that the request was successful and there's no content to return
        }

        // Delete a food
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            await _foodRepository.DeleteAsync(id);
            return NoContent();
        }

        // Save image helper method
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + image.FileName; // Return relative path
        }
       
        //moi them
        
        
        [HttpGet("best-selling")]
        public async Task<ActionResult<IEnumerable<Food>>> GetBestSellingFoods()
        {
            var foods = await _foodRepository.GetAllAsync();
            var bestSellingFoods = foods
                .OrderByDescending(f => f.SoldCount)
                .Take(5); // Lấy 10 món bán chạy nhất
            return Ok(_mapper.Map<List<FoodDTO>>(bestSellingFoods));
        }

    }
}
