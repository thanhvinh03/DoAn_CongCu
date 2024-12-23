using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface IBuffetDetailRepository
    {
        Task<IEnumerable<BuffetDetail>> GetAllBuffetDetailsAsync(int idBuffet); // Lấy tất cả buffet detail
        Task<BuffetDetail> GetBuffetDetailByIdAsync(int id);         // Lấy buffet detail theo Id
        Task CreateBuffetDetailAsync(BuffetDetail buffetDetail);     // Tạo mới buffet detail
        Task UpdateBuffetDetailAsync(BuffetDetail buffetDetail);     // Cập nhật buffet detail
        Task DeleteBuffetDetailAsync(int id);                        // Xóa buffet detail
    }
}
