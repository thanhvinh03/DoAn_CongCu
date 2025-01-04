using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Cashier.Controllers
{
    [Area("Cashier")]
    [Authorize(Roles = SD.Role_Cashier)]
    public class HomeController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _categoryRepository;
        
        public HomeController(IInvoiceRepository invoiceRepository, ITableRepository tableRepository, IFoodRepository foodRepository, IFoodCategoryRepository categoryRepository)
        {
            _invoiceRepository = invoiceRepository;
            _tableRepository = tableRepository;
            _foodRepository = foodRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Code khác để tạo xung đột khi merge
            var allInvoices = await _invoiceRepository.GetAllAsync();
            var allTables = await _tableRepository.GetAllAsync();
            var allFoods = await _foodRepository.GetAllAsync();
            var allCategories = await _categoryRepository.GetAllAsync();

            var totalSales = allInvoices.Sum(i => i.Price);
            var numberOfTables = allTables.Count();
            var numberOfFoods = allFoods.Count();
            var numberOfCategories = allCategories.Count();

            ViewBag.allInvoices = allInvoices;
            ViewBag.totalSales = totalSales;
            ViewBag.numberOfTables = numberOfTables;
            ViewBag.numberOfFoods = numberOfFoods;
            ViewBag.numberOfCategories = numberOfCategories;

            return View();
        }
    }
}
