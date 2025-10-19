using Microsoft.AspNetCore.Mvc;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly CarMaintenanceDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ImageController(CarMaintenanceDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, long vehicleCheckinId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                // Tạo thư mục lưu ảnh
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-checkins");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tạo URL ảnh
                var imageUrl = $"/uploads/vehicle-checkins/{fileName}";

                // Lưu vào database
                var vehicleCheckinImage = new VehicleCheckinImage
                {
                    VehicleCheckinId = vehicleCheckinId,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                _context.VehicleCheckinImages.Add(vehicleCheckinImage);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = new { id = vehicleCheckinImage.Id, imageUrl = imageUrl } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImages(long id)
        {
            try
            {
                var images = await _context.VehicleCheckinImages
                    .Where(img => img.VehicleCheckinId == id)
                    .ToListAsync();

                return Ok(new { success = true, data = images });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
