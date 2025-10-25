using AutoMapper;
using BE.vn.fpt.edu.DTOs;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BE.vn.fpt.edu.services
{
    public interface ITypeComponentService
    {
        Task<IEnumerable<TypeComponentResponseDto>> GetAllAsync(bool onlyActive = false);
        Task<TypeComponentResponseDto> GetByIdAsync(long id);
        Task<TypeComponentResponseDto> CreateAsync(TypeComponentRequestDto dto);
        Task UpdateAsync(long id, TypeComponentRequestDto dto);
        Task SetActiveAsync(long id, bool isActive);
    }

    public class TypeComponentService : ITypeComponentService
    {
        private readonly ITypeComponentRepository _repo;
        private readonly IMapper _mapper;

        public TypeComponentService(ITypeComponentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TypeComponentResponseDto>> GetAllAsync(bool onlyActive = false)
        {
            var list = await _repo.GetAllAsync(onlyActive);
            return _mapper.Map<IEnumerable<TypeComponentResponseDto>>(list);
        }

        public async Task<TypeComponentResponseDto> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TypeComponentResponseDto>(entity);
        }

        public async Task<TypeComponentResponseDto> CreateAsync(TypeComponentRequestDto dto)
        {
            var entity = _mapper.Map<TypeComponent>(dto);
            var created = await _repo.CreateAsync(entity);
            return _mapper.Map<TypeComponentResponseDto>(created);
        }

        public async Task UpdateAsync(long id, TypeComponentRequestDto dto)
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
            await _repo.SetActiveAsync(id, isActive);
        }
    }
}