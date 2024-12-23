using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface ITableRepository
    {
        Task<IEnumerable<Table>> GetAllAsync();
        Task<Table> GetByIdAsync(int id);
        Task<Table> AddAsync();
        Task UpdateAsync(Table table);
        Task DeleteAsync(int id);
        //mới thêm
        //Task<Table> UpdateStatus(int id, bool incluDeleted = false);
    }
}
