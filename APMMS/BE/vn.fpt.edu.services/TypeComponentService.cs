using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.TypeComponent;
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

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(bool onlyActive = false)
        {
            var list = await _repo.GetAllAsync(onlyActive);
            return _mapper.Map<IEnumerable<ResponseDto>>(list);
        }

        public async Task<ResponseDto> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            // business validations (unique name, etc.) can be added here
            var entity = _mapper.Map<TypeComponent>(dto);
            var created = await _repo.CreateAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("TypeComponent not found");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            await _repo.UpdateAsync(existing);
        }

        public async Task SetActiveAsync(long id, bool isActive)
        {
            // business checks (e.g., prevent disabling when components exist) can be added here
            await _repo.SetActiveAsync(id, isActive);
        }
    }
}