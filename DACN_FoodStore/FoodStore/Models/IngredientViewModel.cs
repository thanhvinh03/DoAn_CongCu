using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Models
{
    public class IngredientViewModel
    {
        public Ingredients Ingredient { get; set; } // Đối tượng Ingredient
        public IEnumerable<Food> Foods { get; set; } // Danh sách các món ăn để chọn
    }

}
