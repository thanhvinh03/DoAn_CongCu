using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFTableRepository : ITableRepository
    {
        private readonly ApplicationDbContext _context;
        public EFTableRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Table>> GetAllAsync()
        {

            return await _context.Tables.ToListAsync();

        }
        public async Task<Table> GetByIdAsync(int id)
        {
            return await _context.Tables.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Table> AddAsync()
        {
            var newTable = new Table();
            _context.Tables.Add(newTable);
            await _context.SaveChangesAsync();
            return newTable;
        }
        public async Task UpdateAsync(Table table)
        {
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
        }
        //mới thêm 
        /*public async Task<Table> UpdateStatus(int id, bool incluDeleted = false)
        {
            var table = await _context.Tables.Where(obj => obj.Id == id && obj.IsDeleted == incluDeleted).FirstOrDefaultAsync();
            if (table.Status == false)
            {
                table.Status = true;
                await _context.SaveChangesAsync();
            }
            else if (table.Status == true)
            {
                table.Status = false;
                await _context.SaveChangesAsync();
            }
            return null;
        }*/

        
    }
    
}
