using FoodStore.Models;
using FoodStore.Repositories;
using FoodStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly ApplicationDbContext _context;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly OrderService _orderService;

        public OrderController(IOrderRepository orderRepository,
                               ITableRepository tableRepository,
                               ApplicationDbContext context,
                               IIngredientRepository ingredientRepository,
                               OrderService orderService
                               )
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _ingredientRepository = ingredientRepository;
            _context = context;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var order = await _orderRepository.GetListOrder();
            ViewBag.orderList = order;
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> OrderAccepted()
        {
            var order = await _orderRepository.GetListOrderAccept();
            ViewBag.orderList = order;
            return View(order);
        }


        [HttpGet]
        public async Task<IActionResult> Accept(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            var table = await _tableRepository.GetByIdAsync(order.TableId);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            var orderDetails = await _context.OrderDetails
                                              .Where(od => od.OrderId == id)
                                              .ToListAsync();

            var totalIngredientsNeeded = new Dictionary<int, int>();
            foreach (var orderDetail in orderDetails)
            {
                var foodIngredients = await _context.FoodIngredient
                                                    .Where(fi => fi.FoodId == orderDetail.FoodId)
                                                    .ToListAsync();

                foreach (var foodIngredient in foodIngredients)
                {
                    if (totalIngredientsNeeded.ContainsKey(foodIngredient.IngredientId))
                    {
                        totalIngredientsNeeded[foodIngredient.IngredientId] += foodIngredient.QuantityRequired * orderDetail.Quantity;
                    }
                    else
                    {
                        totalIngredientsNeeded[foodIngredient.IngredientId] = foodIngredient.QuantityRequired * orderDetail.Quantity;
                    }
                }
            }

            var insufficientIngredients = new List<string>();
            foreach (var ingredientId in totalIngredientsNeeded.Keys)
            {
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                var requiredAmount = totalIngredientsNeeded[ingredientId];
                var availableAmount = ingredient?.Quantity ?? 0;

                if (ingredient == null || availableAmount < requiredAmount)
                {
                    var ingredientName = ingredient?.Name ?? "Không xác định";
                    var shortage = requiredAmount - availableAmount;
                    insufficientIngredients.Add($"Nguyên liệu '{ingredientName}' không đủ. Cần: {requiredAmount}, Có sẵn: {availableAmount}, Còn thiếu: {shortage}");
                }
            }

            if (insufficientIngredients.Any())
            {
                ViewBag.ErrorMessage = string.Join("<br/>", insufficientIngredients);
                return View("ErrorAccept");
            }

            order.Status = true;
            table.Status = 1;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        [HttpGet]
        [Route("/Admin/Order/Detail/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            // Lấy thông tin đơn hàng
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            // Lấy chi tiết đơn hàng
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(id);
            ViewBag.OrderDetails = orderDetail;

            // Lấy danh sách nguyên liệu
            var ingredients = await _ingredientRepository.GetAllIngredientsAsync();

            // Kiểm tra số lượng nguyên liệu đã lấy
            Console.WriteLine($"Ingredients count: {ingredients.Count()}"); // Kiểm tra số lượng nguyên liệu
            ViewBag.Ingredients = ingredients;

            ViewBag.Id = order.Id;
            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> SaveRecipe(int id, List<FoodIngredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                if (ingredient.IngredientId != 0) // Bỏ qua các hàng trống
                {
                    ingredient.FoodId = id;
                    await _ingredientRepository.AddFoodIngredientAsync(ingredient);
                }
            }

            return RedirectToAction("Detail", new { id });
        }


        [HttpGet]
        public async Task<IActionResult> DetailAccepted(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(id);
            ViewBag.OrderDetails = orderDetail;

            if (order == null)
            {
                return NotFound();
            }
            ViewBag.Id = order.Id;
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Denied(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
