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

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = _mapper.Map<Component>(dto);
            var created = await _repo.AddAsync(entity);
            var response = _mapper.Map<ResponseDto>(created);
            // map nested names if needed
            response.TypeComponentName = created.TypeComponent?.Name;
            response.BranchName = created.Branch?.Name;
            return response;
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            await _repo.DisableEnableAsync(id, statusCode);
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10, long? branchId = null, long? typeComponentId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(page, pageSize, branchId, typeComponentId, statusCode, search);
            var mapped = _mapper.Map<IEnumerable<ResponseDto>>(list);
            // enrich nested names
            foreach (var dst in mapped)
            {
                var src = System.Linq.Enumerable.FirstOrDefault(list, x => x.Id == dst.Id);
                if (src != null)
                {
                    dst.TypeComponentName = src.TypeComponent?.Name;
                    dst.BranchName = src.Branch?.Name;
                }
            }
            return mapped;
        }

        public async Task<int> GetTotalCountAsync(long? branchId = null, long? typeComponentId = null, string? statusCode = null, string? search = null)
        {
            return await _repo.GetTotalCountAsync(branchId, typeComponentId, statusCode, search);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            var dto = _mapper.Map<ResponseDto>(e);
            dto.TypeComponentName = e.TypeComponent?.Name;
            dto.BranchName = e.Branch?.Name;
            return dto;
        }

        public async Task<ResponseDto?> UpdateAsync(RequestDto dto)
        {
            if (!dto.Id.HasValue) return null;
            var exist = await _repo.GetByIdAsync(dto.Id.Value);
            if (exist == null) return null;

            exist.Code = dto.Code;
            exist.Name = dto.Name;
            exist.ImageUrl = dto.ImageUrl;
            exist.QuantityStock = dto.QuantityStock;
            exist.UnitPrice = dto.UnitPrice;
            exist.PurchasePrice = dto.PurchasePrice;
            exist.BranchId = dto.BranchId;
            exist.TypeComponentId = dto.TypeComponentId;
            exist.StatusCode = dto.StatusCode;

            var updated = await _repo.UpdateAsync(exist);
            var resp = _mapper.Map<ResponseDto>(updated);
            resp.TypeComponentName = updated.TypeComponent?.Name;
            resp.BranchName = updated.Branch?.Name;
            return resp;
        }
    }
}
