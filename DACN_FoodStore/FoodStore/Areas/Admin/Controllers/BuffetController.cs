using FoodStore.Models;
using FoodStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BuffetController : Controller
    {
        private readonly IBuffetRepository _buffetRepository;
        private readonly IBuffetDetailRepository _buffetDetailRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly ApplicationDbContext _context;

        public BuffetController(IBuffetRepository buffetRepository, IFoodRepository foodRepository, ApplicationDbContext context, IBuffetDetailRepository buffetDetailRepository)
        {
            _buffetRepository = buffetRepository;
            _foodRepository = foodRepository;
            _context = context;
            _buffetDetailRepository = buffetDetailRepository;
        }

        // Hiển thị danh sách tất cả các buffet
        public async Task<IActionResult> Index()
        {
            var buffets = await _buffetRepository.GetAllBuffetsAsync();
            return View(buffets);
        }

        // Hiển thị form thêm buffet
        public async Task<IActionResult> Create()
        {
            var listFood = await _foodRepository.GetAllAsync();
            ViewBag.ListFood = listFood; // Truyền danh sách món ăn vào ViewBag
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Buffet buffet, int[] selectedFoods, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                buffet.Status = buffet.Status;
                // Kiểm tra và lưu hình ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/buffets");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ImageFile.FileName)}";
                    var filePath = Path.Combine(imagePath, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Gán đường dẫn ảnh vào Buffet
                        buffet.Image = $"/images/buffets/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Lỗi khi upload hình ảnh: {ex.Message}");
                        return View(buffet);
                    }
                }

                // Lưu buffet vào cơ sở dữ liệu
                try
                {
                    await _buffetRepository.CreateBuffetAsync(buffet);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm buffet: {ex.Message}");
                    return View(buffet);
                }

                // Thêm các BuffetDetail
                if (selectedFoods != null && selectedFoods.Any())
                {
                    foreach (var foodId in selectedFoods)
                    {
                        var buffetDetail = new BuffetDetail
                        {
                            BuffetId = buffet.Id, // Id được gán sau khi lưu buffet
                            FoodId = foodId
                        };

                        try
                        {
                            await _buffetRepository.AddBuffetDetailAsync(buffetDetail);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Lỗi khi thêm BuffetDetail: {ex.Message}");
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách món ăn
            var listFood = await _foodRepository.GetAllAsync();
            ViewBag.ListFood = listFood;
            return View(buffet);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy buffet từ cơ sở dữ liệu
            var buffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (buffet == null)
            {
                return NotFound();
            }

            var buffetItem = await _buffetDetailRepository.GetAllBuffetDetailsAsync(buffet.Id);
            // Lấy danh sách các FoodId đã chọn từ BuffetDetails
            var selectedFoodIds = buffetItem.Select(bd => bd.FoodId).ToList() ?? new List<int>();

            // Truyền danh sách FoodIds đã chọn vào ViewBag
            ViewBag.SelectedFoodIds = selectedFoodIds;

            // Lấy danh sách tất cả các món ăn
            var listFood = await _foodRepository.GetAllAsync();
            ViewBag.ListFood = listFood;

            return View(buffet);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(int id, Buffet buffet, int[] selectedFoods, IFormFile ImageFile)
        {

            var existingBuffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (existingBuffet == null)
            {
                return NotFound();
            }

            // Cập nhật các thông tin buffet
            existingBuffet.Name = buffet.Name;
            existingBuffet.Price = buffet.Price;
            existingBuffet.Description = buffet.Description;
            existingBuffet.Status = buffet.Status;

            // Xử lý hình ảnh mới (nếu có)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/buffets");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ImageFile.FileName)}";
                var filePath = Path.Combine(imagePath, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn hình ảnh mới
                    existingBuffet.Image = $"/images/buffets/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi upload hình ảnh: {ex.Message}");
                    return View(buffet);
                }
            }

            // Cập nhật buffet vào cơ sở dữ liệu
            await _buffetRepository.UpdateBuffetAsync(existingBuffet);

            // Cập nhật các món ăn trong BuffetDetail
            if (selectedFoods != null)
            {
                var buffetItem = await _buffetDetailRepository.GetAllBuffetDetailsAsync(id);
                var currentFoodIds = buffetItem.Select(bd => bd.FoodId).ToList();
                var foodsToRemove = currentFoodIds.Except(selectedFoods).ToList();
                var foodsToAdd = selectedFoods.Except(currentFoodIds).ToList();

                // Xóa các món ăn không còn được chọn
                foreach (var foodId in foodsToRemove)
                {
                    var buffetDetail = buffetItem.FirstOrDefault(bd => bd.FoodId == foodId);
                    if (buffetDetail != null)
                    {
                        _context.BuffetDetails.Remove(buffetDetail);
                    }
                }

                // Thêm các món ăn mới
                foreach (var foodId in foodsToAdd)
                {
                    var buffetDetail = new BuffetDetail
                    {
                        BuffetId = existingBuffet.Id,
                        FoodId = foodId
                    };
                    _context.BuffetDetails.Add(buffetDetail);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Chuyển hướng sau khi cập nhật
        }

        // Hiển thị giao diện xác nhận xóa buffet
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var buffet = await _buffetRepository.GetBuffetByIdAsync(id);
            if (buffet == null)
            {
                return NotFound();
            }
            return View(buffet);
        }

        // Xóa buffet
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _buffetRepository.DeleteBuffetAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi xóa buffet: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

    }

}

