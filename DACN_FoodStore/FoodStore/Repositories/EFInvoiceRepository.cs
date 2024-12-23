using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFInvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public EFInvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.Include("Order").ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllPaidAsync()
        {
            return await _context.Invoices.Where(x => x.Status == true).ToListAsync();
        }


        public async Task<Invoice> GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices.Include("Order").FirstOrDefaultAsync(x => x.Id == id);
            return invoice;
        }

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice, Table table)
        {
            _context.Invoices.Update(invoice);
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoices = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoices);
            await _context.SaveChangesAsync();
        }

        //mới thêm
        public async Task<Invoice> UpdateAsync(int id, bool incluDeleted = false)
        {
            var invoiceUpdate = await _context.Invoices.FirstOrDefaultAsync(obj => obj.Id == id && obj.IsDeleted == incluDeleted && obj.Status == false);
            if (invoiceUpdate != null)
            {
                invoiceUpdate.Status = true;
                await _context.SaveChangesAsync();
                return invoiceUpdate;
            }
            return null;
        }
        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            if (invoice != null)
            {
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return invoice;
            }
            return null;

        }
             public async Task<List<Invoice>> GetAllPaidAsync(int id, bool incluDeleted = false)
        {
            var list = await _context.Invoices.Where(obj =>obj.Status ==true && obj.IsDeleted == incluDeleted).ToListAsync();
            return list;
        }
    }
}
