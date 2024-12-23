using FoodStore.Models;

public interface IOrderDetailRepository
{
    Task<IEnumerable<OrderDetail>> GetAllAsync();
    Task<OrderDetail> GetByIdAsync(int id);
    Task AddAsync(OrderDetail order);
    Task UpdateAsync(OrderDetail order);
    Task DeleteAsync(int id);

    // Phương thức đếm số món ăn chưa hoàn thành
    Task<int> CountUnfinishedAsync();

    // Phương thức đếm số món ăn chờ bàn giao
    Task<int> CountWaitingForDeliveryAsync();

    // Ko đủ nguyên liệu
    Task<int> CountNotReadyAsync();

    Task<OrderDetail> GetOrderDetailAsync(int orderId, int foodId);
    Task UpdateOrderDetailAsync(OrderDetail orderDetail);
    Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId); // Lấy tất cả chi tiết của đơn hàng
}
