using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class QRCodeController : Controller
    {
        private readonly IQRCodeRepository _qrCodeRepository;
        private readonly ApplicationDbContext _context;

        public QRCodeController(IQRCodeRepository qrCodeRepository, ApplicationDbContext context)
        {
            _qrCodeRepository = qrCodeRepository;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        // Lấy thông tin QR code theo ID
        public async Task<IActionResult> Details(int id)
        {
            var qrCode = await _qrCodeRepository.GetQRCodeByIdAsync(id);
            if (qrCode == null)
            {
                return NotFound();
            }
            return View(qrCode);
        }

        // Thêm một QR code mới
        [HttpPost]
        public async Task<IActionResult> Create(QRCode qrCode)
        {
            if (ModelState.IsValid)
            {
                await _qrCodeRepository.AddQRCodeAsync(qrCode);
                return RedirectToAction(nameof(Index));
            }
            return View(qrCode);
        }

        // Cập nhật trạng thái QR code
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool status)
        {
            var result = await _qrCodeRepository.UpdateQRCodeStatusAsync(id, status);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}

