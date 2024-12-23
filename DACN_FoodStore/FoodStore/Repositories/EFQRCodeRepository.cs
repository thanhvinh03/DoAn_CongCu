using FoodStore.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Repositories
{
    public class EFQRCodeRepository : IQRCodeRepository
    {
        private readonly ApplicationDbContext _context;
        public EFQRCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // Lấy QR code theo ID
        public async Task<QRCode> GetQRCodeByIdAsync(int id)
        {
            return await _context.QRCodes.FindAsync(id);
        }

        //Lấy tất cả QR code
        public async Task<IEnumerable<QRCode>> GetAllQRCodesAsync()
        {
            return await _context.QRCodes.ToListAsync();
        }

       
        //Thêm 1 QR code mới
        public async Task<QRCode> AddQRCodeAsync(QRCode qrCode)
        {
            _context.QRCodes.Add(qrCode);
            await _context.SaveChangesAsync();
            return qrCode;
        }

       
        //Cập nhật QR code
        public async Task<QRCode> UpdateQRCodeAsync(QRCode qrCode)
        {
            _context.QRCodes.Update(qrCode);
            await _context.SaveChangesAsync();
            return qrCode;
        }

       
        //Xóa QR code theo ID
        public async Task<bool> DeleteQRCodeAsync(int id)
        {
            var qrCode = await _context.QRCodes.FindAsync(id);
            if (qrCode == null) return false;

            _context.QRCodes.Remove(qrCode);
            await _context.SaveChangesAsync();
            return true;
        }

        // Cập nhật trạng thái của QR code
        public async Task<bool> UpdateQRCodeStatusAsync(int id, bool status)
        {
            var qrCode = await _context.QRCodes.FindAsync(id);
            if (qrCode == null) return false;

            qrCode.Status = status;
            _context.QRCodes.Update(qrCode);
            await _context.SaveChangesAsync();
            return true;
        }


        // Kiểm tra xem QR code có tồn tại trong hệ thống không
        public async Task<bool> IsQRCodeAccessibleAsync(string key)
        {
            // Kiểm tra xem QR code với ID này có tồn tại trong cơ sở dữ liệu không
            var qrCode = await _context.QRCodes.FirstOrDefaultAsync(x => x.Key.Equals(key));
            
            
            return qrCode.Status == false; // Kiểm tra URL không trống
        }



        //dọc được qr của bản đó là mới nhất để nó kiểm tra chuyển status
        public async Task<QRCode> GetQRCodeByIdTableAsync(int idTable)
        {
            return await _context.QRCodes
                .Where(x => x.IdTable == idTable)
                .OrderByDescending(x => x.CreatedAt) // Sắp xếp theo thời gian tạo, giả sử có cột CreatedAt
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateQrStatusBefore(int idTable)
        {
            try
            {
                // Lấy danh sách các QR codes cần cập nhật
                var qrCodesToUpdate = await _context.QRCodes
                    .Where(qr => qr.IdTable == idTable)
                    .ToListAsync();

                if (qrCodesToUpdate.Count == 0)
                {
                    return false; // Không có bản ghi nào cần cập nhật
                }

                // Cập nhật status cho các QR codes
                foreach (var qrCode in qrCodesToUpdate)
                {
                    qrCode.Status = true;
                }

                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();
                return true; // Cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (nếu cần)
                Console.Error.WriteLine($"Lỗi khi cập nhật status: {ex.Message}");
                return false;
            }
        }
    }
}

