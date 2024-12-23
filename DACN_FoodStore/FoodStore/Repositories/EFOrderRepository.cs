using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            if (order != null)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int id, bool incluDeleted = false)
        {
            try
            {
                if (id != 0)
                {
                    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.StatusPay == false);
                    if (order != null)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Order>> GetListOrder(bool incluDeleted = false)
        {
            try
            {
                var order = await _context.Orders.Where(obj => obj.Status == false && obj.StatusPay == false).ToListAsync();
                return order.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Order>> GetListOrderAccept(bool incluDeleted = false)
        {
            var order = await _context.Orders.Where(obj => obj.Status == true && obj.StatusPay == false).ToListAsync();
            return order.ToList();
        }

        public async Task<List<OrderDetail>> GetListOrderDetailsByIdOrder(int id)
        {
            var orderDetails = await _context.OrderDetails.Where(o => o.OrderId == id).Include(x => x.Food).ToListAsync();
            return orderDetails.ToList();
        }

        public async Task<Order> GetOrderPaidAsync(int idTable, bool incluDeleted = false)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(obj => obj.TableId == idTable && obj.StatusPay == true);
            return order;
        }

        public async Task<Order> GetOrderAcceptedById(int id, bool incluDeleted = false)
        {
            if (id == 0)
            {
                return null;
            }
            var order = await _context.Orders.FirstOrDefaultAsync(obj => obj.Id == id);
            order.StatusPay = true;
            return order;
        }

        public async Task<Order> GetOrderById(int id, bool incluDeleted = false)
        {
            if (id == 0)
            {
                return null;
            }
            var order = await _context.Orders.FirstOrDefaultAsync(obj => obj.Id == id);
            return order;
        }

        public async Task<Order> UpdateAsync(int id, bool incluDeleted = false)
        {
            var orderUpdate = await _context.Orders.FirstOrDefaultAsync(obj => obj.Id == id && obj.Status == false);
            var tableUpdate = await _context.Tables.FirstOrDefaultAsync(obj => obj.Id == orderUpdate.TableId && obj.Status == 0);
            if (orderUpdate != null && tableUpdate != null)
            {
                orderUpdate.Status = true;
                tableUpdate.Status = 1;
                await _context.SaveChangesAsync();
                return orderUpdate;
            }
            return null;
        }

        public async Task<Order> UpdatePayAsync(int id, bool incluDeleted = false)
        {
            var orderUpdate = await _context.Orders.FirstOrDefaultAsync(obj => obj.Id == id && obj.Status == true);
            if (orderUpdate != null)
            {
                orderUpdate.StatusPay = true;
                await _context.SaveChangesAsync();
                return orderUpdate;
            }
            return null;
        }

        public async Task<List<OrderDetail>> GetAcceptedOrderDetails() // Phương thức đã thêm
        {
            return await _context.OrderDetails
                .Include(od => od.Food)
                .Include(od => od.Order)
                .Where(od => od.Order.Status == true) // Chỉ lấy các món trong đơn hàng đã tiếp nhận
                .ToListAsync();
        }
    

        public Task<List<Order>> GetListOrder(List<int> idStore, bool incluDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}
