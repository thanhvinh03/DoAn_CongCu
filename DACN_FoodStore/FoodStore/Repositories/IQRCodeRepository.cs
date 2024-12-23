using FoodStore.Models;

namespace FoodStore.Repositories
{
    public interface IQRCodeRepository
    {
        Task<QRCode> GetQRCodeByIdAsync(int id);    
        Task<IEnumerable<QRCode>> GetAllQRCodesAsync();
        Task<QRCode> AddQRCodeAsync(QRCode qrCode);
        Task<QRCode> UpdateQRCodeAsync(QRCode qrCode);
        Task<bool> DeleteQRCodeAsync(int id);
        Task<bool> UpdateQRCodeStatusAsync(int id, bool status);
        Task<bool> IsQRCodeAccessibleAsync(string key);

        Task<QRCode> GetQRCodeByIdTableAsync(int idTable);

        Task<bool> UpdateQrStatusBefore(int idTable);
    }
}
