using FoodStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodStore.Repositories
{
    public class EFFoodIngredientRepository : IFoodIngredientRepository
    {
        private readonly ApplicationDbContext _context;

        public EFFoodIngredientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodIngredient>> GetAllAsync()
        {
            // Lấy tất cả FoodIngredient bao gồm thông tin Food và Ingredient
            return await _context.FoodIngredient
                .Include(fi => fi.Food)
                .Include(fi => fi.Ingredients)
                .ToListAsync();
        }

        public async Task<FoodIngredient> GetByIdAsync(int foodId, int ingredientId)
        {
            // Lấy FoodIngredient theo FoodId và IngredientId
            return await _context.FoodIngredient
                .FirstOrDefaultAsync(fi => fi.FoodId == foodId && fi.IngredientId == ingredientId);
        }

        public async Task AddAsync(FoodIngredient foodIngredient)
        {
            // Thêm FoodIngredient mới vào cơ sở dữ liệu
            _context.FoodIngredient.Add(foodIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FoodIngredient foodIngredient)
        {
            // Cập nhật FoodIngredient
            _context.FoodIngredient.Update(foodIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int foodId, int ingredientId)
        {
            // Tìm FoodIngredient theo FoodId và IngredientId
            var foodIngredient = await GetByIdAsync(foodId, ingredientId);
            if (foodIngredient != null)
            {
                // Xóa FoodIngredient khỏi cơ sở dữ liệu
                _context.FoodIngredient.Remove(foodIngredient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FoodIngredient>> GetIngredientsByFoodIdAsync(int foodId)
        {
            return await _context.FoodIngredient
                .Where(fi => fi.FoodId == foodId)
                .ToListAsync();
        }

        public async Task<bool> DeductIngredientsAsync(int foodId, int quantity)
        {
            // Lấy danh sách nguyên liệu cho món ăn
            var foodIngredients = await _context.FoodIngredient
                .Where(fi => fi.FoodId == foodId)
                .Include(fi => fi.Ingredients) // Bao gồm thông tin nguyên liệu
                .ToListAsync();

            foreach (var foodIngredient in foodIngredients)
            {
                var ingredient = await _context.Ingredients.FindAsync(foodIngredient.IngredientId);
                if (ingredient != null)
                {
                    if (ingredient.Quantity >= quantity)
                    {
                        ingredient.Quantity -= quantity; // Giảm số lượng nguyên liệu
                        _context.Ingredients.Update(ingredient); // Cập nhật nguyên liệu
                    }
                    else
                    {
                        return false; // Không đủ số lượng
                    }
                }
            }

            await _context.SaveChangesAsync(); // Lưu thay đổi
            return true; // Thành công
        }


        public async Task<List<FoodIngredient>> GetFoodIngredientsByFoodIdAsync(int foodId)
        {
            return await _context.FoodIngredient
                .Where(fi => fi.FoodId == foodId)
                .ToListAsync();
        }



    }
}
