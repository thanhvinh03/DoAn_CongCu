using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFFoodRepository : IFoodRepository
    {
        private readonly ApplicationDbContext _context;
        public EFFoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Food>> GetAllAsync()
        {
            // Lấy danh sách món ăn và tính toán số lượng đã bán (SoldCount)
            var foodsWithSoldCount = await _context.Foods
                .Where(x => x.IsDeleted == false)
                .Select(f => new Food
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    Image = f.Image,
                    Type = f.Type,
                    FoodCategoryId = f.FoodCategoryId,
                    SoldCount = _context.OrderDetails
                        .Where(od => od.FoodId == f.Id)
                        .Sum(od => od.Quantity), // Tính tổng số lượng đã bán
                    FoodCategorys = f.FoodCategorys
                })
                .ToListAsync();

            return foodsWithSoldCount;
        }


        public async Task<Food> GetByIdAsync(int id)
        {
            return await _context.Foods.Include("FoodCategorys").FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task<IEnumerable<Food>> GetListFoodByIdAsync(int idCategory)
        {
            return await _context.Foods
                        .Include("FoodCategorys")
                        .Where(f => f.FoodCategoryId == idCategory && f.IsDeleted == false)
                        .ToListAsync();

        }
        public async Task AddAsync(Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            food.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        //moithem bestselling
        public async Task<IEnumerable<Food>> GetBestSellingFoodsAsync(int limit)
        {
            return await _context.Foods
                .Where(f => f.IsDeleted == false) // Chỉ lấy món ăn chưa bị xóa
                .Select(f => new Food
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    Image = f.Image,
                    FoodCategoryId = f.FoodCategoryId,
                    SoldCount = _context.OrderDetails
                                        .Where(od => od.FoodId == f.Id)
                                        .Sum(od => od.Quantity) // Tính tổng số lượng đã bán
                })
                .OrderByDescending(f => f.SoldCount) // Sắp xếp theo số lượng bán
                .Take(limit) // Lấy số lượng giới hạn
                .ToListAsync();
        }
        //// Phương thức tính và cập nhật SoldCount cho món ăn
        //public async Task UpdateSoldCountAsync(int foodId)
        //{
        //    // Tính tổng số lượng đã bán cho món ăn từ bảng OrderDetails
        //    var soldCount = await _context.OrderDetails
        //        .Where(od => od.FoodId == foodId) // Lọc theo FoodId
        //        .SumAsync(od => od.Quantity); // Tính tổng số lượng bán (Quantity)

        //    // Tìm món ăn trong bảng Food và cập nhật SoldCount
        //    var food = await _context.Foods.FindAsync(foodId);
        //    if (food != null)
        //    {
        //        food.SoldCount = soldCount; // Cập nhật SoldCount
        //        await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        //    }
        //}
        //public async Task AddOrderAsync(Order order)
        //{
        //    // Thêm đơn hàng
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    // Cập nhật SoldCount cho các món ăn trong đơn hàng
        //    foreach (var orderDetail in order.OrderDetails)
        //    {
        //        await UpdateSoldCountAsync(orderDetail.FoodId);
        //    }
        //}






    }
}
