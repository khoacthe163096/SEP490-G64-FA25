using AutoMapper;
using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace BE.vn.fpt.edu.services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _db;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, CarMaintenanceDbContext db)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _db = db;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(e => _mapper.Map<EmployeeResponseDto>(e));
        }

        public async Task<EmployeeResponseDto?> GetByIdAsync(long id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return null;
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        // ✅ CHỈNH Ở ĐÂY: tự sinh Code theo Role
        public async Task<EmployeeResponseDto> CreateAsync(EmployeeRequestDto dto)
        {
            var employee = _mapper.Map<User>(dto);

            // Sinh mã tự động nếu chưa có
            var prefix = await GetRolePrefixAsync(dto.RoleId);
            employee.Code = await GenerateUniqueEmployeeCodeAsync(prefix);

            employee.CreatedDate = DateTime.Now;
            employee.IsDelete = false;
            
            // ✅ Set StatusCode default là ACTIVE nếu không được set
            employee.StatusCode = string.IsNullOrWhiteSpace(dto.StatusCode) ? "ACTIVE" : dto.StatusCode;

            try
            {
                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                // Check if it's a username unique constraint violation - this is the username constraint
                throw new ArgumentException($"Tên đăng nhập '{dto.Username}' đã tồn tại. Vui lòng chọn tên khác.");
            }

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto?> UpdateAsync(long id, EmployeeRequestDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return null;

            // Store current values before mapping to preserve them if they are null/empty
            var currentPassword = employee.Password;
            var currentRoleId = employee.RoleId;
            var currentBranchId = employee.BranchId;
            
            _mapper.Map(dto, employee);
            
            // If password is empty or whitespace, keep the original password (don't update password)
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                employee.Password = currentPassword;
            }
            
            // If roleId is null, keep the original roleId
            if (dto.RoleId == null || dto.RoleId == 0)
            {
                employee.RoleId = currentRoleId;
            }
            
            // If branchId is null, keep the original branchId
            if (dto.BranchId == null || dto.BranchId == 0)
            {
                employee.BranchId = currentBranchId;
            }
            
            employee.LastModifiedDate = DateTime.Now;

            try
            {
                await _employeeRepository.UpdateAsync(employee);
                await _employeeRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                // Check if it's a username unique constraint violation - this is the username constraint
                throw new ArgumentException($"Tên đăng nhập '{dto.Username}' đã tồn tại. Vui lòng chọn tên khác.");
            }

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return false;

            await _employeeRepository.SoftDeleteAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return true;
        }

        // 🆕 Lấy prefix theo Role (ưu tiên theo Role.Name trong DB)
        private async Task<string> GetRolePrefixAsync(long? roleId)
        {
            if (roleId == null) return "EM"; // fallback

            var role = await _db.Roles.AsNoTracking()
                                      .Where(r => r.Id == roleId.Value)
                                      .Select(r => r.Name)
                                      .FirstOrDefaultAsync();

            var name = role?.Trim().ToLowerInvariant() ?? string.Empty;

            return name switch
            {
                "consulter" => "CL",
                "accountant" => "AC",
                "technician" => "TC",
                "warehouse keeper" => "WK",
                _ => "EM"
            };
        }

        // 🆕 Sinh mã duy nhất: PREFIX + 5 số ngẫu nhiên (vd: CL04218)
        private async Task<string> GenerateUniqueEmployeeCodeAsync(string prefix)
        {
            const int digits = 5;
            var rnd = new Random();

            for (int i = 0; i < 50; i++)
            {
                var number = rnd.Next(0, (int)Math.Pow(10, digits)).ToString($"D{digits}");
                var code = $"{prefix}{number}";

                var exists = await _db.Users.AsNoTracking().AnyAsync(u => u.Code == code);
                if (!exists) return code;
            }

            // Fallback (hầu như không bao giờ xảy ra)
            return $"{prefix}{DateTime.UtcNow:HHmmssff}";
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync(string? status = null, int? roleId = null, string? roleName = null)
        {
            var query = _employeeRepository.GetAll()
                .Include(e => e.Role)
                .Include(e => e.Branch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "active")
                    query = query.Where(e => e.IsDelete == false);
                else if (status.ToLower() == "inactive")
                    query = query.Where(e => e.IsDelete == true);
            }

            if (roleId.HasValue)
                query = query.Where(e => e.RoleId == roleId.Value);

            if (!string.IsNullOrEmpty(roleName))
                query = query.Where(e => e.Role.Name.ToLower().Contains(roleName.ToLower()));

            var employees = await query.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeResponseDto>> FilterAsync(bool? isDelete, long? roleId)
        {
            var query = _employeeRepository.GetAll();

            if (isDelete.HasValue)
                query = query.Where(e => e.IsDelete == isDelete.Value);

            if (roleId.HasValue)
                query = query.Where(e => e.RoleId == roleId.Value);

            var employees = await query.ToListAsync();
            return employees.Select(e => _mapper.Map<EmployeeResponseDto>(e));
        }

        public async Task<EmployeeProfileDto?> GetProfileAsync(long userId)
        {
            var employee = await _employeeRepository.GetByIdAsync(userId);
            if (employee == null) return null;

            var dto = new EmployeeProfileDto
            {
                Username = employee.Username,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Gender = employee.Gender,
                Image = employee.Image,
                BranchName = employee.Branch?.Name,
                RoleName = employee.Role?.Name
            };

            return dto;
        }

        public async Task<EmployeeProfileDto?> UpdateProfileAsync(long userId, UpdateProfileDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(userId);
            if (employee == null) return null;

            if (!string.IsNullOrEmpty(dto.FirstName)) employee.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) employee.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Email)) employee.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Phone)) employee.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Gender)) employee.Gender = dto.Gender;
            if (!string.IsNullOrEmpty(dto.Image)) employee.Image = dto.Image;
            employee.LastModifiedDate = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return await GetProfileAsync(userId);
        }

        public async Task<object> GetWithFiltersAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, long? roleId = null)
        {
            var employees = await _employeeRepository.GetWithFiltersAsync(page, pageSize, search, status, roleId);
            var totalCount = await _employeeRepository.GetTotalCountAsync(search, status, roleId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            return new
            {
                success = true,
                data = _mapper.Map<List<EmployeeResponseDto>>(employees),
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                currentPage = page,
                totalCount = totalCount
            };
        }
        
        /// <summary>
        /// ✅ Cập nhật Status của Employee (ACTIVE hoặc INACTIVE)
        /// </summary>
        public async Task<EmployeeResponseDto?> UpdateStatusAsync(long id, string statusCode)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new ArgumentException("Employee not found");
            
            // Validate status
            if (statusCode != "ACTIVE" && statusCode != "INACTIVE")
                throw new ArgumentException($"Invalid status: {statusCode}. Only ACTIVE or INACTIVE is allowed.");
            
            employee.StatusCode = statusCode;
            employee.LastModifiedDate = DateTime.Now;
            
            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            
            return _mapper.Map<EmployeeResponseDto>(employee);
        }
    }
}
