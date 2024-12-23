using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class FoodCategoryController : Controller
    {
        private readonly IFoodCategoryRepository _foodcategoryRepository;
        public FoodCategoryController( IFoodCategoryRepository foodcategoryRepository)
        {
            _foodcategoryRepository = foodcategoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var foodcategory = await _foodcategoryRepository.GetAllAsync();
            return View(foodcategory);
        }
        public async Task<IActionResult> Display(int id)
        {
            var foodcategory = await _foodcategoryRepository.GetByIdAsync(id);
            if (foodcategory == null)
            {
                return NotFound();
            }
            return View(foodcategory);
        }
        [HttpGet] 
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(FoodCategory loaiThucPham)
        {
            if (!ModelState.IsValid)
            {
                await _foodcategoryRepository.AddAsync(loaiThucPham);
                return RedirectToAction(nameof(Index));
            }
            return View(loaiThucPham);
        }

        // Thêm [HttpGet] như này
        public async Task<IActionResult> Update(int id)
        {
            var foodcategory = await _foodcategoryRepository.GetByIdAsync(id);
            if (foodcategory == null)
            {
                return NotFound();
            }
            return View(foodcategory);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, FoodCategory foodcategory)
        {
            if (id != foodcategory.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                await _foodcategoryRepository.UpdateAsync(foodcategory);
                return RedirectToAction(nameof(Index));
            }
            return View(foodcategory);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var foodcategory = await _foodcategoryRepository.GetByIdAsync(id);
            if (foodcategory == null)
            {
                return NotFound();
            }
            return View(foodcategory);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _foodcategoryRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
