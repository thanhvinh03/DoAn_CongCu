using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFBuffetRepository : IBuffetRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBuffetRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Buffet>> GetAllBuffetsAsync()
        {
            return await _context.Buffets
                .Include(b => b.BuffetDetails)
                .ThenInclude(d => d.Food)
                .ToListAsync();
        }

        public async Task<Buffet> GetBuffetByIdAsync(int id)
        {
            return await _context.Buffets.FindAsync(id);
        }

        public async Task CreateBuffetAsync(Buffet buffet)
        {
            await _context.Buffets.AddAsync(buffet);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateBuffetAsync(Buffet buffet)
        {
            _context.Buffets.Update(buffet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBuffetAsync(int id)
        {
            var buffet = await _context.Buffets.FindAsync(id);
            if (buffet != null)
            {
                _context.Buffets.Remove(buffet);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddBuffetDetailAsync(BuffetDetail buffetDetail)
        {
            _context.BuffetDetails.Add(buffetDetail);
            await _context.SaveChangesAsync();
        }
    }
}

