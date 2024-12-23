using FoodStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodStore.Repositories
{
    public class EFIngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _context;

        public EFIngredientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingredients>> GetAllAsync()
        {
            // Lấy tất cả nguyên liệu chưa bị xóa
            return await _context.Ingredients.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<Ingredients> GetByIdAsync(int id)
        {
            // Lấy nguyên liệu theo ID nếu chưa bị xóa
            return await _context.Ingredients.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task AddAsync(Ingredients ingredient)
        {
            // Thêm nguyên liệu mới vào cơ sở dữ liệu
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ingredients ingredient)
        {
            // Cập nhật nguyên liệu
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Tìm nguyên liệu theo ID
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                // Đánh dấu nguyên liệu là đã xóa
                ingredient.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Ingredients>> GetAllIngredientsAsync()
        {
            // Truy vấn lấy danh sách nguyên liệu chưa bị xóa
            var ingredients = await _context.Ingredients
                                            .Where(i => !i.IsDeleted)
                                            .ToListAsync();

            return ingredients;
        }

        public async Task AddFoodIngredientAsync(FoodIngredient foodIngredient)
        {
            _context.FoodIngredient.Add(foodIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeductIngredientsAsync(Dictionary<int, int> ingredientsToDeduct)
        {
            // Bắt đầu một transaction để đảm bảo tất cả hoặc không có gì được thay đổi
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra từng nguyên liệu có đủ trong kho không
                    foreach (var ingredientEntry in ingredientsToDeduct)
                    {
                        var ingredientId = ingredientEntry.Key;
                        var quantityToDeduct = ingredientEntry.Value;

                        // Lấy nguyên liệu từ database
                        var ingredient = await _context.Ingredients.FindAsync(ingredientId);

                        // Nếu không đủ nguyên liệu, rollback và trả về false
                        if (ingredient == null || ingredient.Quantity < quantityToDeduct)
                        {
                            Console.WriteLine($"Not enough ingredient ID {ingredientId}. Required: {quantityToDeduct}, Available: {ingredient?.Quantity ?? 0}");
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    // Nếu đủ nguyên liệu, tiến hành trừ kho
                    foreach (var ingredientEntry in ingredientsToDeduct)
                    {
                        var ingredientId = ingredientEntry.Key;
                        var quantityToDeduct = ingredientEntry.Value;

                        var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                        ingredient.Quantity -= quantityToDeduct;
                        _context.Ingredients.Update(ingredient);
                    }

                    // Lưu thay đổi và commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true; // Thành công
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi, rollback và báo lỗi
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error deducting ingredients: {ex.Message}");
                    return false;
                }
            }
        }


    }
}
