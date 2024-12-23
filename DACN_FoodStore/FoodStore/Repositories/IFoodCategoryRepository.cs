using FoodStore.Models;
namespace FoodStore.Repositories
{
    public interface IFoodCategoryRepository
    {
        Task<IEnumerable<FoodCategory>> GetAllAsync();
        Task<FoodCategory> GetByIdAsync(int id);
        Task AddAsync(FoodCategory foodcategory);
        Task UpdateAsync(FoodCategory foodcategory);
        Task DeleteAsync(int id);
    }
}
