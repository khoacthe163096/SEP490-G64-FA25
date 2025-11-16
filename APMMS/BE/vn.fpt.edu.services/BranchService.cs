using AutoMapper;
using BE.vn.fpt.edu.DTOs.Branch;
using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _dbContext;
        private const long DIRECTOR_ROLE_ID = 2; // Giả sử RoleId = 2 là Giám đốc chi nhánh

        public BranchService(IBranchRepository branchRepository, IMapper mapper, CarMaintenanceDbContext dbContext)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BranchResponseDto>> GetAllAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Select(b => _mapper.Map<BranchResponseDto>(b));
        }

        public async Task<BranchResponseDto?> GetByIdAsync(long id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return null;
            
            var branchDto = _mapper.Map<BranchResponseDto>(branch);
            
            // Load director information
            var director = await GetDirectorAsync(id);
            branchDto.Director = director;
            
            return branchDto;
        }

        public async Task<BranchResponseDto> CreateAsync(BranchRequestDto dto)
        {
            var branch = _mapper.Map<Branch>(dto);
            await _branchRepository.AddAsync(branch);
            await _branchRepository.SaveChangesAsync();
            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<BranchResponseDto?> UpdateAsync(long id, BranchRequestDto dto)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return null;

            branch.Name = dto.Name;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;
            branch.LaborRate = dto.LaborRate;

            await _branchRepository.UpdateAsync(branch);
            await _branchRepository.SaveChangesAsync();

            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return false;

            // Kiểm tra xem chi nhánh có đang được sử dụng không
            var isInUse = await _branchRepository.IsBranchInUseAsync(id);
            if (isInUse)
            {
                throw new InvalidOperationException("Không thể xóa chi nhánh này vì đang được sử dụng trong hệ thống");
            }

            await _branchRepository.DeleteAsync(branch);
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeResponseDto?> GetDirectorAsync(long branchId)
        {
            // Tìm giám đốc: User có BranchId = branchId, RoleId = 2 (Giám đốc), StatusCode = "ACTIVE"
            var director = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => 
                    u.BranchId == branchId && 
                    u.RoleId == DIRECTOR_ROLE_ID && 
                    u.StatusCode == "ACTIVE" &&
                    (u.IsDelete == null || u.IsDelete == false));

            if (director == null) return null;

            return _mapper.Map<EmployeeResponseDto>(director);
        }

        public async Task<bool> ChangeDirectorAsync(long branchId, long newDirectorId)
        {
            // Kiểm tra branch có tồn tại không
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chi nhánh");
            }

            // Kiểm tra nhân viên mới có tồn tại không
            var newDirector = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == newDirectorId);
            
            if (newDirector == null)
            {
                throw new KeyNotFoundException("Không tìm thấy nhân viên");
            }

            // Kiểm tra nhân viên mới có cùng chi nhánh không
            if (newDirector.BranchId != branchId)
            {
                throw new InvalidOperationException("Nhân viên không thuộc chi nhánh này");
            }

            // Tìm và set giám đốc cũ thành INACTIVE
            var oldDirector = await _dbContext.Users
                .FirstOrDefaultAsync(u => 
                    u.BranchId == branchId && 
                    u.RoleId == DIRECTOR_ROLE_ID && 
                    u.StatusCode == "ACTIVE" &&
                    (u.IsDelete == null || u.IsDelete == false));

            if (oldDirector != null)
            {
                oldDirector.StatusCode = "INACTIVE";
                oldDirector.LastModifiedDate = DateTime.Now;
            }

            // Set nhân viên mới thành giám đốc
            newDirector.RoleId = DIRECTOR_ROLE_ID;
            newDirector.StatusCode = "ACTIVE";
            newDirector.LastModifiedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

