using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFBuffetDetailRepository : IBuffetDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBuffetDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả buffet detail
        public async Task<IEnumerable<BuffetDetail>> GetAllBuffetDetailsAsync(int idBuffet)
        {
            return await _context.BuffetDetails
                                 .Include(bd => bd.Buffet) // Bao gồm thông tin Buffet
                                 .Include(bd => bd.Food)
                                 .Where(x => x.BuffetId == idBuffet)// Bao gồm thông tin Food
                                 .ToListAsync();
        }

        // Lấy buffet detail theo Id
        public async Task<BuffetDetail> GetBuffetDetailByIdAsync(int id)
        {
            return await _context.BuffetDetails
                                 .Include(bd => bd.Buffet)
                                 .Include(bd => bd.Food)
                                 .FirstOrDefaultAsync(bd => bd.Id == id);
        }

        // Tạo mới buffet detail
        public async Task CreateBuffetDetailAsync(BuffetDetail buffetDetail)
        {
            _context.BuffetDetails.Add(buffetDetail);
            await _context.SaveChangesAsync();  // Lưu vào cơ sở dữ liệu
        }

        // Cập nhật buffet detail
        public async Task UpdateBuffetDetailAsync(BuffetDetail buffetDetail)
        {
            _context.BuffetDetails.Update(buffetDetail);
            await _context.SaveChangesAsync();  // Lưu vào cơ sở dữ liệu
        }

        // Xóa buffet detail
        public async Task DeleteBuffetDetailAsync(int id)
        {
            var buffetDetail = await _context.BuffetDetails.FindAsync(id);
            if (buffetDetail != null)
            {
                _context.BuffetDetails.Remove(buffetDetail);
                await _context.SaveChangesAsync();  // Lưu vào cơ sở dữ liệu
            }
        }
    }
}

