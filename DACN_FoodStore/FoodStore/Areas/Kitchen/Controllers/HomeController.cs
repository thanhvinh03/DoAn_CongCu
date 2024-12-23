using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FoodStore.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    [Authorize(Roles = SD.Role_Kitchen)]
    public class HomeController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IFoodCategoryRepository _categoryRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public HomeController(
            IInvoiceRepository invoiceRepository,
            ITableRepository tableRepository,
            IFoodRepository foodRepository,
            IFoodCategoryRepository categoryRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _invoiceRepository = invoiceRepository;
            _tableRepository = tableRepository;
            _foodRepository = foodRepository;
            _categoryRepository = categoryRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        // GET: Home/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get data from repositories
            var invoices = await _invoiceRepository.GetAllAsync();
            var tables = await _tableRepository.GetAllAsync();
            var foods = await _foodRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var totalRevenue = invoices.Sum(invoice => invoice.Price); // Tổng doanh thu
            var tableCount = tables.Count();
            var foodCount = foods.Count();
            var categoryCount = categories.Count();
            // Get counts for unfinished and waiting for delivery orders
            var unfinishedCount = await _orderDetailRepository.CountUnfinishedAsync(); // Món ăn chưa hoàn thành
            var waitingForDeliveryCount = await _orderDetailRepository.CountWaitingForDeliveryAsync(); // Món ăn chờ bàn giao
            var notReadyCount = await _orderDetailRepository.CountNotReadyAsync(); // Món ăn chờ bàn giao

            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TableCount = tableCount;
            ViewBag.FoodCount = foodCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.UnfinishedCount = unfinishedCount;
            ViewBag.WaitingForDeliveryCount = waitingForDeliveryCount;
            ViewBag.NotReadyCount = notReadyCount;
            ViewBag.Invoices = invoices;

            return View();
        }
    }
}
