using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class TypeComponentService : ITypeComponentService
    {
        private readonly ITypeComponentRepository _repository;
        private readonly IMapper _mapper;

        public TypeComponentService(ITypeComponentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseDto>>(entities);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = _mapper.Map<TypeComponent>(dto);
            entity.StatusCode ??= "ACTIVE";

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.BranchId = dto.BranchId;
            entity.StatusCode = dto.StatusCode;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.StatusCode = entity.StatusCode == "ACTIVE" ? "INACTIVE" : "ACTIVE";
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}