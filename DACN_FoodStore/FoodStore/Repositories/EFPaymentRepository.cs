using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFPaymentRepository : IPaymentRepository
    {

        private readonly ApplicationDbContext _context;
        public EFPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {

            return await _context.Payments.ToListAsync();

        }
        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

    }
}
