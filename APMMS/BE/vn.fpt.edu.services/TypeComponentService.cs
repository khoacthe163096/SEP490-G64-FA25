using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class TypeComponentService : ITypeComponentService
    {
        private readonly ITypeComponentRepository _repo;
        private readonly IMapper _mapper;

        public TypeComponentService(ITypeComponentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = _mapper.Map<TypeComponent>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            await _repo.DisableEnableAsync(id, statusCode);
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(branchId, statusCode, search);
            return _mapper.Map<IEnumerable<ResponseDto>>(list);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto?> UpdateAsync(RequestDto dto)
        {
            if (!dto.Id.HasValue) return null;
            var exist = await _repo.GetByIdAsync(dto.Id.Value);
            if (exist == null) return null;

            // Map changed fields
            exist.Name = dto.Name;
            exist.Description = dto.Description;
            exist.BranchId = dto.BranchId;
            exist.StatusCode = dto.StatusCode;

            var updated = await _repo.UpdateAsync(exist);
            return _mapper.Map<ResponseDto>(updated);
        }
    }
}