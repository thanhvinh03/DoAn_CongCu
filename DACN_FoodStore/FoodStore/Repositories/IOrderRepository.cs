using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface IOrderRepository
    {
        public Task<Order> CreateAsync(Order order);
        public Task<List<Order>> GetListOrder(bool incluDeleted = false);
        public Task<Order> UpdateAsync(int id, bool incluDeleted = false);
        public Task<Order> UpdatePayAsync(int id, bool incluDeleted = false);
        public Task<bool> DeleteAsync(int id, bool incluDeleted = false);
        public Task<Order> GetOrderById(int id, bool incluDeleted = false);
        public Task<List<OrderDetail>> GetListOrderDetailsByIdOrder(int id);
        public Task<Order> GetOrderAcceptedById(int id, bool incluDeleted = false);
        public Task<List<Order>> GetListOrderAccept( bool incluDeleted = false);
        public Task<Order> GetOrderPaidAsync(int idTable, bool incluDeleted = false);
        Task<List<OrderDetail>> GetAcceptedOrderDetails();
    }
}
