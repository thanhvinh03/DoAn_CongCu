using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
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

        //chỗ này cần sửa lại 

        public async Task<IActionResult> Index()
        {
            var order = await _orderRepository.GetListOrder();
            ViewBag.orderList = order;
            return View(order);
        }

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
            var orderupdate = await _orderRepository.UpdateAsync(id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("/Employee/Order/Detail/{id:int}")]
        public async Task<IActionResult> Detail(int id)
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
        /*[HttpGet]
        public async Task<IActionResult> GetListOrderAccept(int id)
        {
            var order = await _orderRepository.GetListOrderAccept(id);
            ViewBag.IdStore = 1;
            return View(order);
        }*/
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

        public async Task<IActionResult> FoodDelivery()
        {
            var acceptedOrderDetails = await _orderRepository.GetAcceptedOrderDetails();

            acceptedOrderDetails = acceptedOrderDetails
                .Where(od => !od.Order.StatusPay && od.Status == 2)
                .ToList();

            return View("FoodDelivery", acceptedOrderDetails);
        }

        [HttpPost]
        public async Task<IActionResult> MoveToDelivered(int orderId, int foodId, int status)
        {
            // TODO use service to query instead of context
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order) // Bao gồm thông tin đơn hàng
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);

            if (orderDetail != null)
            {
                orderDetail.Status = status; // Cập nhật trạng thái món ăn
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("FoodDelivery"); // Hoặc chuyển tới một trang khác nếu cần
        }
    }
}
