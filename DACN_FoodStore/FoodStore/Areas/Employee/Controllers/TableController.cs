using FoodStore.Models;
using FoodStore.Repositories;
using FoodStore.Services;
using IronBarCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using NuGet.DependencyResolver;
using Supabase.Storage;
using System.Security.Claims;

namespace FoodStore.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class TableController : Controller
    {
        private readonly ITableRepository _banRepository;
        private readonly IQRCodeRepository _qrRepository;
        private readonly ISupabaseService _supabase;
        private readonly ApplicationDbContext _context;
        public TableController(ITableRepository banRepository, IQRCodeRepository qrRepository, ISupabaseService supabase, ApplicationDbContext context)
        {
            _banRepository = banRepository;
            _context = context;
            _qrRepository = qrRepository;
            _supabase = supabase;
        }
        public async Task<IActionResult> Index()
        {
            var bans = await _banRepository.GetAllAsync();
            return View(bans);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Thực hiện thêm bàn mới vào cơ sở dữ liệu
            var table = await _banRepository.AddAsync();
            await GenerateNewQRAsync(table.Id);
                return RedirectToAction("Index");
        } 
        public async Task<IActionResult> Display(int id)
        {
            var ban = await _banRepository.GetByIdAsync(id);
            if (ban == null)
            {
                return NotFound();
            }
            return View(ban);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var ban = await _banRepository.GetByIdAsync(id);
            if (ban == null)
            {
                return NotFound();
            }
            return View(ban);
        }
        // Xử lý cập nhật bàn
        [HttpPost]
        public async Task<IActionResult> Update(int id, Table ban)
        {
            if (id != ban.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var banupdate = await _banRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                // Cập nhật các thông tin khác của sản phẩm
                banupdate.Id = ban.Id;
                banupdate.Status = ban.Status;
                await _banRepository.UpdateAsync(banupdate);
                return RedirectToAction(nameof(Index));

            }
            return View(ban);
        }
     
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _banRepository.DeleteAsync(id);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReloadQR(int idTable)
        {
            var ban = await _banRepository.GetByIdAsync(idTable);
            await GenerateQRAsync(idTable);
            return RedirectToAction("Display", new { id = idTable });
        }



        private async Task GenerateQRAsync(int idTable)
        {
            string key = Guid.NewGuid().ToString();
            string url = $"http://localhost:5173/order/{idTable}/{key}";
            string fileName = $"{idTable}.png";

           
            GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(url, 250);
            barcode.SetMargins(10);

            
            string tempPath = Path.GetTempFileName();
            barcode.SaveAsImage(tempPath);

       
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempPath);

            var storage = _supabase.GetClient().Storage;
            var bucket = storage.From("QRTable"); 
            string supabasePath = $"{fileName}";
         
            try
            {
                var deleteResponse = await storage.From("QRTable").Remove(supabasePath);

                var response = await storage
                    .From("QRTable")
                    .Update(fileBytes, supabasePath);

                   
            }
            catch (Exception ex)
            {
             
                throw new Exception($"Error uploading file to Supabase: {ex.Message}");
            }
            await _qrRepository.UpdateQrStatusBefore(idTable);

           
            QRCode qrCode = new QRCode
            {
                Url = url,
                Status = false,
                ImagePath = $"qr-codes/{fileName}", 
                Key = key,
                IdTable = idTable,
            };

            await _qrRepository.AddQRCodeAsync(qrCode);

        }

        private async Task GenerateNewQRAsync(int idTable)
        {
            string key = Guid.NewGuid().ToString();
            string url = $"http://localhost:5173/order/{idTable}/{key}";
            string fileName = $"{idTable}.png";

           
            GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(url, 250);
            barcode.SetMargins(10);

            
            string tempPath = Path.GetTempFileName();
            barcode.SaveAsImage(tempPath);

          
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempPath);

            var storage = _supabase.GetClient().Storage;
            var bucket = storage.From("QRTable"); // Replace with your bucket name
            string supabasePath = $"{fileName}";
           
            try
            {
                var response = await storage
                    .From("QRTable")
                    .Upload(fileBytes, supabasePath);
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Error uploading file to Supabase: {ex.Message}");
            }


            
            QRCode qrCode = new QRCode
            {
                Url = url,
                Status = false,
                ImagePath = $"qr-codes/{fileName}", 
                Key = key,
                IdTable = idTable,
            };

            await _qrRepository.AddQRCodeAsync(qrCode);

        }
    }
}




