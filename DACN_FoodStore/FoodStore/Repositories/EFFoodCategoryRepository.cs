using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFFoodCategoryRepository : IFoodCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFFoodCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodCategory>> GetAllAsync()
        {
            return await _context.FoodCategories.Where(x => x.IsDeleted == false).ToListAsync();
        }

        public async Task<FoodCategory> GetByIdAsync(int id)
        {
            return await _context.FoodCategories.FindAsync(id);
        }

        public async Task AddAsync(FoodCategory foodCategory)
        {
            _context.FoodCategories.Add(foodCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FoodCategory foodCategory)
        {
            _context.FoodCategories.Update(foodCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var foodCategory = await _context.FoodCategories.FindAsync(id);
            foodCategory.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
