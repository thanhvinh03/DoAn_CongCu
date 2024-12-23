using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface IInvoiceRepository
    {
        // Get all Invoice 
        Task<IEnumerable<Invoice>> GetAllAsync();

        // Get all invoice paid
        Task<IEnumerable<Invoice>> GetAllPaidAsync();
        Task<Invoice> GetByIdAsync(int id);
        Task<Invoice> AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice,Table table);
        Task DeleteAsync(int id);
        //mới thêm

       // public Task<Invoice> UpdateAsync(int id, bool incluDeleted = false);
        //public Task<Invoice> CreateAsync(Invoice invoice);

       // public Task<Invoice> GetInvoicePaidById(int id, bool incluDeleted = false);
    }
}
