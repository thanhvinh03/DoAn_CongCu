using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    [Authorize(Roles = SD.Role_Kitchen)]
    [Route("api/[area]/[controller]")]
    [ApiController] // Đánh dấu controller là một API controller
    public class OrderAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;

        public OrderAPIController(ApplicationDbContext context, IOrderRepository orderRepository, ITableRepository tableRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var order = await _orderRepository.GetListOrder();
            return Ok(order); // Trả về JSON
        }

        [HttpGet]
        [Route("OrderAccepted")]
        public async Task<IActionResult> OrderAccepted()
        {
            var order = await _orderRepository.GetListOrderAccept();
            return Ok(order); // Trả về JSON
        }

        [HttpGet]
        [Route("Accept/{id:int}")]
        public async Task<IActionResult> Accept(int id)
        {
            var order = await _orderRepository.UpdateAsync(id);
            return Ok(order); // Trả về JSON của đơn hàng sau khi chấp nhận
        }

        [HttpGet]
        [Route("Detail/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            // Trả về JSON bao gồm chi tiết đơn hàng và danh sách món ăn
            return Ok(new { Order = order, OrderDetails = orderDetail });
        }

        [HttpGet]
        [Route("DetailAccepted/{id:int}")]
        public async Task<IActionResult> DetailAccepted(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            var orderDetail = await _orderRepository.GetListOrderDetailsByIdOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(new { Order = order, OrderDetails = orderDetail });
        }

        [HttpPost]
        [Route("UpdateOrderDetailsStatus")]
        public async Task<IActionResult> UpdateOrderDetailsStatus([FromBody] Dictionary<string, bool> Statuses, int orderId)
        {
            foreach (var item in Statuses)
            {
                var keys = item.Key.Split(',');
                int orderDetailOrderId = int.Parse(keys[0]);
                int orderDetailFoodId = int.Parse(keys[1]);

                bool isChecked = item.Value;

                var orderDetail = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == orderDetailOrderId && od.FoodId == orderDetailFoodId);

                if (orderDetail != null)
                {
                    orderDetail.Status = isChecked ? 1 : 0;
                }
            }

            await _context.SaveChangesAsync();

            var order = await _orderRepository.GetOrderById(orderId);
            var orderDetails = await _orderRepository.GetListOrderDetailsByIdOrder(orderId);
            return Ok(new { Order = order, OrderDetails = orderDetails });
        }
    }
}
