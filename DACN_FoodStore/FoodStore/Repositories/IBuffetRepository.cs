using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface IBuffetRepository
    {
        Task<IEnumerable<Buffet>> GetAllBuffetsAsync();  // Lấy tất cả buffet
        Task<Buffet> GetBuffetByIdAsync(int id);         // Lấy buffet theo Id
        Task CreateBuffetAsync(Buffet buffet);           // Tạo mới buffet
        Task UpdateBuffetAsync(Buffet buffet);           // Cập nhật buffet
        Task DeleteBuffetAsync(int id);
        Task AddBuffetDetailAsync(BuffetDetail buffetDetail);// Xóa buffet
    }
}
