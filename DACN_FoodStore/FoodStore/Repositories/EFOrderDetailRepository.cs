using FoodStore.Models;
using Microsoft.EntityFrameworkCore;


public class EFOrderDetailRepository : IOrderDetailRepository
{
    private readonly ApplicationDbContext _context;

    public EFOrderDetailRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDetail>> GetAllAsync()
    {
        return await _context.OrderDetails.ToListAsync();
    }

    public async Task<OrderDetail> GetByIdAsync(int id)
    {
        return await _context.OrderDetails.FindAsync(id);
    }

    public async Task AddAsync(OrderDetail orderDetail)
    {
        _context.OrderDetails.Add(orderDetail);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderDetail orderDetail)
    {
        _context.OrderDetails.Update(orderDetail);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var orderdetail = await _context.OrderDetails.FindAsync(id);
        _context.OrderDetails.Remove(orderdetail);
        await _context.SaveChangesAsync();
    }

    // Đếm số món ăn chờ bàn giao (giả sử Status = -1 là ko đủ nguyên liệu)
    public async Task<int> CountNotReadyAsync()
    {
        return await _context.OrderDetails.CountAsync(od => od.Status == -1);
    }

    public async Task<OrderDetail> GetOrderDetailAsync(int orderId, int foodId)
    {
        // Lấy một chi tiết đơn hàng theo ID của đơn hàng và món ăn
        return await _context.OrderDetails
            .FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);
    }

    public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
    {
        _context.OrderDetails.Update(orderDetail);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        return await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .ToListAsync();
    }
    public async Task<int> CountUnfinishedAsync()
    {
        return await _context.OrderDetails
            .Where(od => !od.Order.StatusPay)
            .Where(od => od.Status != 2)
            .CountAsync();
    }

    // Thêm phương thức để đếm số lượng món ăn chờ bàn giao
    public async Task<int> CountWaitingForDeliveryAsync()
    {
        return await _context.OrderDetails
            .Where(od => !od.Order.StatusPay && od.Status == 2)
            .CountAsync();
    }
    //public async Task UpdateSoldCountAsync(int foodId)
    //{
    //    var soldCount = await _context.OrderDetails
    //        .Where(od => od.FoodId == foodId)
    //        .SumAsync(od => od.Quantity);

    //    var food = await _context.Foods.FindAsync(foodId);
    //    if (food != null)
    //    {
    //        food.SoldCount = soldCount;
    //        await _context.SaveChangesAsync();
    //    }
    //}
    //public async Task AddOrderDetailAsync(OrderDetail orderDetail)
    //{
    //    _context.OrderDetails.Add(orderDetail);
    //    await _context.SaveChangesAsync();

    //    // Cập nhật SoldCount cho món ăn
    //    await UpdateSoldCountAsync(orderDetail.FoodId);
    //}



}
