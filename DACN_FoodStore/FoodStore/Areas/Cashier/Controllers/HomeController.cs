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

        public HomeController(IInvoiceRepository invoiceRepo, ITableRepository tableRepo, IFoodRepository foodRepo, IFoodCategoryRepository categoryRepo)
        {
            _invoiceRepository = invoiceRepo;
            _tableRepository = tableRepo;
            _foodRepository = foodRepo;
            _categoryRepository = categoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Khác biệt về thứ tự xử lý và cách lấy dữ liệu
            var tables = await _tableRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var foods = await _foodRepository.GetAllAsync();
            var invoices = await _invoiceRepository.GetAllAsync();

            var tableCount = tables.Count();
            var categoryCount = categories.Count();
            var foodCount = foods.Count();
            var totalRevenue = invoices.Sum(i => i.Price);

            ViewBag.tableCount = tableCount;
            ViewBag.categoryCount = categoryCount;
            ViewBag.foodCount = foodCount;
            ViewBag.totalRevenue = totalRevenue;
            ViewBag.invoices = invoices;

            return View();
        }
    }
}
