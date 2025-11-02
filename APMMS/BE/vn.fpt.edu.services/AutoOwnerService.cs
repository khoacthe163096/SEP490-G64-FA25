using AutoMapper;
using BE.vn.fpt.edu.DTOs.AutoOwner;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

using BE.vn.fpt.edu.interfaces;

namespace BE.vn.fpt.edu.services
{
    public class AutoOwnerService : IAutoOwnerService
    {
        private readonly IAutoOwnerRepository _repository;
        private readonly IMapper _mapper;

        public AutoOwnerService(IAutoOwnerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var users = await _repository.GetAllAsync(page, pageSize);
            return _mapper.Map<List<ResponseDto>>(users);
        }

        public async Task<object> GetWithFiltersAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, long? roleId = null)
        {
            var users = await _repository.GetWithFiltersAsync(page, pageSize, search, status, roleId);
            var totalCount = await _repository.GetTotalCountAsync(search, status, roleId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            return new
            {
                success = true,
                data = _mapper.Map<List<ResponseDto>>(users),
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                currentPage = page,
                totalCount = totalCount
            };
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var user = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResponseDto?>(user);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var user = _mapper.Map<User>(dto);

            // Business rules
            user.RoleId = 7; // AutoOwner role
            user.StatusCode = "ACTIVE";
            user.CreatedDate = DateTime.UtcNow;
            user.IsDelete = false;
            
            // Explicitly set Address to ensure it's saved
            user.Address = dto.Address;

            await _repository.CreateAsync(user);
            return _mapper.Map<ResponseDto>(user);
        }

        public async Task<ResponseDto> UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Auto Owner not found.");

            _mapper.Map(dto, existing);
            // Explicitly set Address to ensure it's updated
            existing.Address = dto.Address;
            existing.LastModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<int> GetTotalCountAsync(string? search = null, string? status = null, long? roleId = null)
        {
            return await _repository.GetTotalCountAsync(search, status, roleId);
        }
    }
}
