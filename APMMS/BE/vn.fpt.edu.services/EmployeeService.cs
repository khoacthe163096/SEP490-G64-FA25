using AutoMapper;
using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;


namespace BE.vn.fpt.edu.services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
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

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeRequestDto dto)
        {
            var employee = _mapper.Map<User>(dto);
            employee.CreatedDate = DateTime.Now;
            employee.IsDelete = false;
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto?> UpdateAsync(long id, EmployeeRequestDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return null;

            _mapper.Map(dto, employee);
            employee.LastModifiedDate = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

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
        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync(string? status = null, int? roleId = null, string? roleName = null)
        {
            var query = _employeeRepository.GetAll()
                .Include(e => e.Role)
                .Include(e => e.Branch)
                .AsQueryable();

            // ✅ Filter theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "active")
                    query = query.Where(e => e.IsDelete == false);
                else if (status.ToLower() == "inactive")
                    query = query.Where(e => e.IsDelete == true);
            }

            // ✅ Filter theo RoleId
            if (roleId.HasValue)
            {
                query = query.Where(e => e.RoleId == roleId.Value);
            }

            // ✅ Filter theo RoleName
            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(e => e.Role.Name.ToLower().Contains(roleName.ToLower()));
            }

            var employees = await query.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
        }
        public async Task<IEnumerable<EmployeeResponseDto>> FilterAsync(bool? isDelete, long? roleId)
        {
            var query = _employeeRepository.GetAll(); // IQueryable<User>

            if (isDelete.HasValue)
                query = query.Where(e => e.IsDelete == isDelete.Value);

            if (roleId.HasValue)
                query = query.Where(e => e.RoleId == roleId.Value);

            var employees = await query.ToListAsync();
            return employees.Select(e => _mapper.Map<EmployeeResponseDto>(e));
        }


    }
}


