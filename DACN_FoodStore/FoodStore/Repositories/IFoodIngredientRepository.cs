using FoodStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStore.Repositories
{
    public interface IFoodIngredientRepository
    {
        Task<IEnumerable<FoodIngredient>> GetAllAsync();
        Task<FoodIngredient> GetByIdAsync(int foodId, int ingredientId);
        Task AddAsync(FoodIngredient foodIngredient);
        Task UpdateAsync(FoodIngredient foodIngredient);
        Task DeleteAsync(int foodId, int ingredientId);

        Task<List<FoodIngredient>> GetIngredientsByFoodIdAsync(int foodId);
        Task<List<FoodIngredient>> GetFoodIngredientsByFoodIdAsync(int foodId);
    }
}
