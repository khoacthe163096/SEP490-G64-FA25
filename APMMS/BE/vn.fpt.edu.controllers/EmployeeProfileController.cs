using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;

namespace BE.vn.fpt.edu.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeProfileController : ControllerBase
    {
        private readonly CarMaintenanceDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public EmployeeProfileController(CarMaintenanceDbContext dbContext, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Lấy thông tin profile của chính employee (dựa trên userId từ token)
        /// Cho phép tất cả user đã đăng nhập xem profile của chính mình
        /// </summary>
        [HttpGet("my-profile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                // Lấy userId từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid token: UserId not found" });
                }

                // Lấy thông tin user từ DbContext (không giới hạn role)
                var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .FirstOrDefaultAsync(u => u.Id == userId && (u.IsDelete == false || u.IsDelete == null));

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Map sang EmployeeResponseDto
                var employee = _mapper.Map<EmployeeResponseDto>(user);

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật profile của chính employee (dựa trên userId từ token)
        /// Cho phép employee cập nhật thông tin của chính mình (không cho phép thay đổi roleId, branchId, statusCode)
        /// </summary>
        [HttpPut("my-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] EmployeeRequestDto dto)
        {
            try
            {
                // Lấy userId từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid token: UserId not found" });
                }

                // Lấy user từ DbContext
                var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .Include(u => u.Branch)
                    .FirstOrDefaultAsync(u => u.Id == userId && (u.IsDelete == false || u.IsDelete == null));

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Không cho phép employee thay đổi roleId, branchId, statusCode, code của chính mình
                // Chỉ cho phép cập nhật thông tin cá nhân
                if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Phone)) user.Phone = dto.Phone;
                if (!string.IsNullOrEmpty(dto.Gender)) user.Gender = dto.Gender;
                if (!string.IsNullOrEmpty(dto.Image)) user.Image = dto.Image;
                if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
                if (!string.IsNullOrEmpty(dto.CitizenId)) user.CitizenId = dto.CitizenId;
                if (!string.IsNullOrEmpty(dto.TaxCode)) user.TaxCode = dto.TaxCode;
                
                // Parse Dob từ string format dd-MM-yyyy sang DateOnly
                if (!string.IsNullOrEmpty(dto.Dob))
                {
                    if (DateOnly.TryParseExact(dto.Dob, "dd-MM-yyyy", out var dob))
                    {
                        user.Dob = dob;
                    }
                    else if (DateTime.TryParse(dto.Dob, out var dobDateTime))
                    {
                        user.Dob = DateOnly.FromDateTime(dobDateTime);
                    }
                }
                
                user.LastModifiedDate = DateTime.Now;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                // Map sang EmployeeResponseDto
                var result = _mapper.Map<EmployeeResponseDto>(user);

                return Ok(new { success = true, data = result, message = "Profile updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Upload avatar image to Cloudinary for current user (dựa trên userId từ token)
        /// </summary>
        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "Kích thước file không được vượt quá 5MB" });
                }

                // Lấy userId từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "Invalid token: UserId not found" });
                }

                // Upload to Cloudinary (isAvatar = true để crop thành hình vuông)
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "user-avatars", isAvatar: true);

                // Cập nhật image URL vào database
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Xóa ảnh cũ từ Cloudinary nếu có (optional - có thể bỏ qua để tiết kiệm)
                // if (!string.IsNullOrEmpty(user.Image))
                // {
                //     try
                //     {
                //         await _cloudinaryService.DeleteImageAsync(user.Image);
                //     }
                //     catch
                //     {
                //         // Ignore delete errors
                //     }
                // }

                user.Image = imageUrl;
                user.LastModifiedDate = DateTime.Now;
                
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new { success = true, data = new { imageUrl = imageUrl }, message = "Upload avatar thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}

