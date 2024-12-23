using FoodStore.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRCodeController : ControllerBase
    {
        private readonly IQRCodeRepository _qrCodeRepository;
        public QRCodeController(IQRCodeRepository qrcodeRepository)
        {
            _qrCodeRepository = qrcodeRepository;
        }

        // API kiểm tra QR code có thể truy cập hay không
        [HttpGet("check/{key}")]
        public async Task<IActionResult> CheckQRCodeAccessibility(string key)
        {
            var isAccessible = await _qrCodeRepository.IsQRCodeAccessibleAsync(key);

            if (isAccessible)
            {
                return Ok(new { message = "QR code có thể truy cập.", accept = true });
            }
            else
            {
                return BadRequest(new { message = "QR code không tồn tại hoặc không truy cập được.", accept = false });
            }
        }


    }
}
