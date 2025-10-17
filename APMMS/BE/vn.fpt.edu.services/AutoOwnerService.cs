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

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var user = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResponseDto?>(user);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var user = _mapper.Map<User>(dto);

            // Business rules
            user.RoleId = 7; // Giả định RoleId=3 là AutoOwner, sửa nếu cần
            user.StatusCode = "ACTIVE";
            user.CreatedDate = DateTime.UtcNow;
            user.IsDelete = false;

            await _repository.CreateAsync(user);
            return _mapper.Map<ResponseDto>(user);
        }

        public async Task<ResponseDto> UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Auto Owner not found.");

            _mapper.Map(dto, existing);
            existing.LastModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
