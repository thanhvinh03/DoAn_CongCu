using FoodStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStore.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredients>> GetAllAsync(); // Lấy tất cả nguyên liệu
        Task<Ingredients> GetByIdAsync(int id); // Lấy nguyên liệu theo ID
        Task AddAsync(Ingredients ingredient); // Thêm nguyên liệu mới
        Task UpdateAsync(Ingredients ingredient); // Cập nhật nguyên liệu
        Task DeleteAsync(int id); // Xóa nguyên liệu
        //Task<bool> DeductIngredientsAsync(int ingredientId, int quantity); // Giảm số lượng nguyên liệu
        Task<IEnumerable<Ingredients>> GetAllIngredientsAsync();
        Task AddFoodIngredientAsync(FoodIngredient foodIngredient);

        Task<bool> DeductIngredientsAsync(Dictionary<int, int> ingredientsToDeduct); // Trừ nguyên liệu

    }
}
