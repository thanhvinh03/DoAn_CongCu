using AutoMapper;
using FoodStore.DTO;
using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public OrderController(
            IOrderDetailRepository orderDetailRepository,
            IOrderRepository orderRepository, 
            IMapper mapper,
            IFoodRepository foodRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _foodRepository = foodRepository;
        }

        [HttpGet("/{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<OrderDTO>(order));

        }

        [HttpPost]
        public async Task<ActionResult> Create(OrderDTO orderdto)
        {
            try
            {
                var order = _mapper.Map<Order>(orderdto);
                
                var newOrder = await _orderRepository.CreateAsync(order);
                foreach (var item in orderdto.listOrderDetail)
                {
                    var orderDetail = _mapper.Map<OrderDetail>(item);
                    orderDetail.OrderId = newOrder.Id;
                    await _orderDetailRepository.AddAsync(orderDetail);
                }
                return Ok(newOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
