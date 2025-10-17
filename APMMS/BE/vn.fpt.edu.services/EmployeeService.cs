using AutoMapper;
using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

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
    }
}


