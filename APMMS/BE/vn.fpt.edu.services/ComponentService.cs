using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.Component;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class ComponentService : IComponentService
    {
        private readonly IComponentRepository _repo;
        private readonly IMapper _mapper;

        public ComponentService(IComponentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(bool onlyActive = false, long? typeComponentId = null, long? branchId = null)
        {
            var list = await _repo.GetAllAsync(onlyActive, typeComponentId, branchId);
            return _mapper.Map<IEnumerable<ResponseDto>>(list);
        }

        public async Task<ResponseDto> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            // Business validations (unique code, non-negative price/quantity, FK existence) can be added here
            var entity = _mapper.Map<Component>(dto);
            // ensure default IsActive true
            entity.IsActive = true;
            var created = await _repo.CreateAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new InvalidOperationException("Component not found");

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.UnitPrice = dto.UnitPrice ?? existing.UnitPrice;
            existing.QuantityStock = dto.QuantityStock ?? existing.QuantityStock;
            existing.TypeComponentId = dto.TypeComponentId ?? existing.TypeComponentId;
            existing.BranchId = dto.BranchId ?? existing.BranchId;
            existing.ImageUrl = dto.ImageUrl ?? existing.ImageUrl;

            await _repo.UpdateAsync(existing);
        }

        public async Task SetActiveAsync(long id, bool isActive)
        {
            await _repo.SetActiveAsync(id, isActive);
        }
    }
}
