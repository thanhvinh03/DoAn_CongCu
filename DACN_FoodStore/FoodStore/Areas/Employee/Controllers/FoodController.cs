using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class FoodController : Controller
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _foodcategoryRepository;

        public FoodController(IFoodRepository foodRepository, IFoodCategoryRepository foodcategoryRepository)

        {
            _foodRepository = foodRepository;
            _foodcategoryRepository = foodcategoryRepository;
        }

      
        public async Task<IActionResult> Index()
        {
            var foods = await _foodRepository.GetAllAsync();
            return View(foods);
        }
       
        public async Task<IActionResult> Add()
        {
            var foodcategories = await _foodcategoryRepository.GetAllAsync();
            ViewBag.FoodCategories = new SelectList(foodcategories, "Id", "Name");
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Add(Food food, IFormFile Image)
        {
            if (!ModelState.IsValid)
            {
                if (Image != null)
                {
                    
                    food.Image = await SaveImage(Image);
                }
                await _foodRepository.AddAsync(food);
                return RedirectToAction(nameof(Index));
            }
           
            var foodcategories = await _foodcategoryRepository.GetAllAsync();
            ViewBag.FoodCategories = new SelectList(foodcategories, "Id", "Name");
            return View(food);
        }

        
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); //

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }
        
        public async Task<IActionResult> Display(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return View(food);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            var foodcategories = await _foodcategoryRepository.GetAllAsync();
            ViewBag.FoodCategories = new SelectList(foodcategories, "Id", "Name", food.FoodCategoryId);

            return View(food);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Food food, IFormFile imageUrl)

        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl

            if (id != food.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var existingFood= await

                _foodRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên

                if (imageUrl == null)
                {
                    food.Image = existingFood.Image;
                }
                else
                {
                    // Lưu hình ảnh mới
                    food.Image = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingFood.Name = food.Name;
                existingFood.Price = food.Price;
                existingFood.FoodCategoryId = food.FoodCategoryId;
                existingFood.Image = food.Image;
                existingFood.Status = food.Status;
                existingFood.Type = food.Type;
                await _foodRepository.UpdateAsync(existingFood);
                return RedirectToAction(nameof(Index));
            }
            var foodcategories = await _foodcategoryRepository.GetAllAsync();
            ViewBag.FoodCategories = new SelectList(foodcategories, "Id", "Name");
            return View(food);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return View(food);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _foodRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
