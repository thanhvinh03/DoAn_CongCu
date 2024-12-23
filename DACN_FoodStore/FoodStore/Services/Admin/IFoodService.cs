using FoodStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStore.Services.Admin
{
    public interface IFoodService
    {
        Task<bool> SaveRecipeAsync(int foodId, List<FoodIngredient> ingredients);
    }
}
