using System.Collections.Generic;
using System.Linq; // Thêm vào để sử dụng LINQ
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodStore.Models;
using FoodStore.Repositories;

namespace FoodStore.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    [Authorize(Roles = SD.Role_Kitchen)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;

        public OrderController(ApplicationDbContext context, IOrderRepository orderRepository, ITableRepository tableRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var acceptedOrderDetails = await _orderRepository.GetAcceptedOrderDetails();

            // Lọc các mục chưa thanh toán và chưa hoàn thành
            acceptedOrderDetails = acceptedOrderDetails
                .Where(od => !od.Order.StatusPay && od.Status < 2)
                 .OrderBy(od => od.Order.Created)
                //.OrderBy(od => od.Status)    // Sắp xếp theo Status tăng dần
                //.ThenBy(od => od.FoodId)     // Sau đó sắp xếp theo FoodId
                .ToList();

            return View(acceptedOrderDetails);

        }


        public async Task<IActionResult> OrderAccepted()
        {
            var acceptedOrderDetails = await _orderRepository.GetAcceptedOrderDetails();

            acceptedOrderDetails = acceptedOrderDetails
                .Where(od => !od.Order.StatusPay && od.Status == 2)
                .ToList();

            return View("OrderAccepted", acceptedOrderDetails);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Food)
                .Include(od => od.Order)
                .Where(od => !od.Order.StatusPay) // Lọc các đơn hàng chưa thanh toán
                .Where(od => od.Status < 2) // Lọc ra các món ăn chưa hoàn thành
                .OrderBy(od => od.Order.Created) // Sắp xếp theo thời gian tạo (bỏ qua sắp xếp theo bàn)
                .ToListAsync();

            return PartialView("_OrderDetailsQueue", orderDetails); // Trả về một partial view
        }

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

        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetailStatus(int orderId, int foodId, int status)
        {
            Console.WriteLine($"Order ID: {orderId}, Food ID: {foodId}, Status: {status}");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderDetail = await GetOrderDetailAsync(orderId, foodId);
                    if (orderDetail == null)
                    {
                        Console.WriteLine("Order detail not found");
                        return NotFound("Order detail not found");
                    }

                    // Nếu chuyển từ trạng thái 1 (hoàn thành) về 0 (đang đặt), hoàn lại nguyên liệu
                    if (orderDetail.Status == 1 && status == 0)
                    {
                        Console.WriteLine("Reverting status from 1 to 0. Restoring ingredients to inventory.");
                        await RestoreIngredientsToInventoryAsync(foodId, orderDetail.Quantity);
                        UpdateStatus(orderDetail, status);
                    }
                    else if (ShouldCheckAndDeductIngredients(orderDetail.Status, status))
                    {
                        var hasEnoughIngredients = await CheckAndUpdateIngredientsAsync(foodId, orderDetail.Quantity);

                        if (!hasEnoughIngredients)
                        {
                            Console.WriteLine("Not enough ingredients. Updating status to -1.");
                            UpdateStatus(orderDetail, -1); // Thiếu nguyên liệu
                        }
                        else
                        {
                            Console.WriteLine("Inventory updated successfully for ingredients.");
                            UpdateStatus(orderDetail, status);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No inventory update needed as the status change is not from 0 to 1.");
                        UpdateStatus(orderDetail, status);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    Console.WriteLine("Changes saved to database");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    return StatusCode(500, "Error updating order details.");
                }
            }

            return RedirectToAction("Index");
        }
        //HOÀN NGUYÊN LIỆU
        private async Task RestoreIngredientsToInventoryAsync(int foodId, int quantity)
        {
            var ingredientDetails = await _context.FoodIngredient
                .Where(fi => fi.FoodId == foodId)
                .ToListAsync();

            foreach (var detail in ingredientDetails)
            {
                var ingredient = await _context.Ingredients.FindAsync(detail.IngredientId);
                if (ingredient != null)
                {
                    ingredient.Quantity += detail.QuantityRequired * quantity;
                    Console.WriteLine($"Restored {detail.QuantityRequired * quantity} of {ingredient.Name} to inventory.");
                }
            }

            await _context.SaveChangesAsync();
        }


        // Hàm lấy chi tiết đơn hàng
        private async Task<OrderDetail> GetOrderDetailAsync(int orderId, int foodId)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);
        }

        // Hàm kiểm tra nếu cần trừ nguyên liệu
        private bool ShouldCheckAndDeductIngredients(int currentStatus, int newStatus)
        {
            return currentStatus < 1 && newStatus == 1;
        }

        // Hàm kiểm tra và cập nhật nguyên liệu
        // Hàm kiểm tra và cập nhật nguyên liệu, xem xét số lượng món được đặt
        private async Task<bool> CheckAndUpdateIngredientsAsync(int foodId, int quantity)
        {
            // Lấy công thức của món ăn
            var formulaList = await _context.FoodIngredient
                .Where(fi => fi.FoodId == foodId)
                .ToListAsync();

            foreach (var formula in formulaList)
            {
                // Tính tổng lượng nguyên liệu cần cho số lượng món đã đặt
                int totalRequiredQuantity = formula.QuantityRequired * quantity;

                var ingredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Id == formula.IngredientId);

                if (ingredient == null)
                {
                    Console.WriteLine($"Ingredient with ID {formula.IngredientId} not found in inventory.");
                    continue;
                }

                // Kiểm tra nếu không đủ số lượng
                if (ingredient.Quantity < totalRequiredQuantity)
                {
                    Console.WriteLine($"Not enough ingredient ID {ingredient.Id}. Required: {totalRequiredQuantity}, Available: {ingredient.Quantity}");
                    return false; // Thiếu nguyên liệu
                }
            }

            // Nếu đủ nguyên liệu, tiến hành trừ kho
            foreach (var formula in formulaList)
            {
                int totalRequiredQuantity = formula.QuantityRequired * quantity;
                var ingredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Id == formula.IngredientId);

                ingredient.Quantity -= totalRequiredQuantity;
                _context.Ingredients.Update(ingredient);
            }

            return true;
        }

        // Hàm cập nhật trạng thái của chi tiết đơn hàng
        private void UpdateStatus(OrderDetail orderDetail, int newStatus)
        {
            Console.WriteLine($"Updating status for Order Detail - Order ID: {orderDetail.OrderId}, Food ID: {orderDetail.FoodId} to Status: {newStatus}");
            orderDetail.Status = newStatus;
        }

        [HttpPost]
        public async Task<IActionResult> MoveToOngoing(int orderId, int foodId, int status)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order) // Bao gồm thông tin đơn hàng
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);

            bool notYetDelivered = orderDetail.Status < 3;
            if (orderDetail != null && notYetDelivered)
            {
                orderDetail.Status = status; // Cập nhật trạng thái món ăn
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OrderAccepted"); // Hoặc chuyển tới một trang khác nếu cần
        }
    }
}
