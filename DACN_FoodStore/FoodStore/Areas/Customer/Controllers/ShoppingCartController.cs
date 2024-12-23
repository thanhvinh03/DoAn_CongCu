using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodStore.Extensions;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Http;

namespace FoodStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AllowAnonymous]
    public class ShoppingCartController : Controller
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ITableRepository _tableRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public ShoppingCartController(IFoodRepository foodRepository,
                                      ITableRepository tableRepository,
                                      ApplicationDbContext context,
                                      UserManager<ApplicationUser> userManager,
                                      IOrderRepository orderRepository)
        {
            _foodRepository = foodRepository;
            _tableRepository = tableRepository;
            _context = context;
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        [Route("/customer/Order/ViewCart/{idTable:int}")]
        public IActionResult ViewCart(int idTable)
        {
            ViewBag.idTable = idTable;

            var temp = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            return View(temp);
        }
        public async Task<IActionResult> AddToCart(int idTable, int foodId, int quantity)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var food = await GetProductFromDatabase(foodId);
            if (food == null)
            {
                return RedirectToAction("Index", "ShoppingCart", new { idTable });

            }
            var cartItem = new CartItem
            {
                FoodId = foodId,
                Name = food.Name,
                Price = food.Price,
                Quantity = quantity,
                Picture = food.Image,
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index", "ShoppingCart", new { idTable });

        }
        //---------------------End Cart-----------------------------//
        [HttpGet]
        [Route("/customer/order/{idTable:int}")]
        public async Task<IActionResult> Index(int idTable)
        {
            ViewBag.idTable = idTable;
            var table = await _tableRepository.GetByIdAsync(idTable);
            if (table.Status == 0)
            {
                var food = await _foodRepository.GetAllAsync();
                var groupedFood = food.GroupBy(f => f.FoodCategorys).ToList();
                
                return View(groupedFood);
            }
            else
            {
                return RedirectToAction("NotFound", "ShoppingCart");
            }
        }

        [HttpGet]

        public async Task<IActionResult> NotFound()
        {
            return View();

        }

        // Các actions khác...
        private async Task<Food> GetProductFromDatabase(int foodId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var food = await _foodRepository.GetByIdAsync(foodId);
            return food;
        }
        [HttpGet]
        public IActionResult RemoveFromCart(int idTable, int foodId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(foodId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("ViewCart", "ShoppingCart", new { idTable });
        } 
        [HttpPost]

        public IActionResult ClearCart()
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            // Kiểm tra nếu giỏ hàng không null
            if (cart != null)
            {
                // Xóa tất cả các sản phẩm trong giỏ hàng
                cart.Clear();

                // Cập nhật giỏ hàng trong Session
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            // Chuyển hướng đến trang giỏ hàng
            return RedirectToAction("NotFound", "ShoppingCart");
        }


        [HttpPost]
        [Route("/customer/Order/UpdateQuantity")]
        public IActionResult UpdateQuantity(int foodId, int quantity)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            // Kiểm tra nếu giỏ hàng không null
            if (cart != null)
            {
                // Lấy ra mục sản phẩm cần cập nhật số lượng
                var cartItem = cart.Items.FirstOrDefault(item => item.FoodId == foodId);

                // Kiểm tra nếu mục sản phẩm tồn tại
                if (cartItem != null)
                {
                    // Cập nhật số lượng mới cho mục sản phẩm
                    cartItem.Quantity = quantity;

                    // Cập nhật giỏ hàng trong Session
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            // Chuyển hướng đến trang giỏ hàng
            return Ok();
        }

        [HttpPost]
        [Route("customer/shoppingcart/submit")]
        public async Task<IActionResult> SubmitOrderAsync(int idTable)
        {
            try
            {

                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart == null || !cart.Items.Any())
                {
                    return RedirectToAction("Index");
                }
                Order order = new Order();
                order.TableId = idTable;
               
                order.TotalPrice = (double) cart.Items.Sum(i => i.Price * i.Quantity);
                order.Status = false;
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    FoodId = i.FoodId,
                    Quantity = i.Quantity,

                }).ToList();
                await _orderRepository.CreateAsync(order);

                HttpContext.Session.Remove("Cart");

                return RedirectToAction("Index", "ShoppingCart", new { idTable });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult DeleteFromCart( int foodId, int idTable)
        {

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            var itemToDelete = cart.Items.FirstOrDefault(obj => obj.FoodId == foodId);
            if (itemToDelete != null)
            {
                cart.Items.Remove(itemToDelete);
                if (cart != null)
                {
                    HttpContext.Session.SetObjectAsJson("Cart", cart);

                }
            }
            return RedirectToAction("ViewCart", "ShoppingCart", new {  idTable });
        }


    }

}

