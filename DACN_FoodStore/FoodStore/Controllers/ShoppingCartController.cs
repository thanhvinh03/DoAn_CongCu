using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodStore.Extensions;
using FoodStore.Models;
using FoodStore.Repositories;

namespace FoodStore.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ShoppingCartController(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager, IFoodRepository
        productRepository)
        {
            _foodRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            return View(new Order());
        }
        public async Task<IActionResult> AddToCart(int foodId, int quantity)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var food = await GetProductFromDatabase(foodId);
            var cartItem = new CartItem
            {
                FoodId = foodId,
                Name = food.Name,
                Price = food.Price,
                Quantity = quantity
            };
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new
            ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new
            ShoppingCart();
            return View(cart);
        }
        // Các actions khác...
        private async Task<Food> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _foodRepository.GetByIdAsync(productId);
            return product;
        }
        public IActionResult RemoveFromCart(int productId)
        {
            var cart =
            HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
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
            return RedirectToAction("Index", "ShoppingCart");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            // Kiểm tra nếu giỏ hàng không null
            if (cart != null)
            {
                // Lấy ra mục sản phẩm cần cập nhật số lượng
                var cartItem = cart.Items.FirstOrDefault(item => item.FoodId == productId);

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
            return RedirectToAction("Index", "ShoppingCart");
        }


    }

}

