using AutoMapper;
using BE.vn.fpt.edu.DTOs.ServicePackage;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class ServicePackageService : IServicePackageService
    {
        private readonly IServicePackageRepository _repo;
        private readonly CarMaintenanceDbContext _context;
        private readonly IMapper _mapper;

        public ServicePackageService(IServicePackageRepository repo, CarMaintenanceDbContext context, IMapper mapper)
        {
            _repo = repo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = new ServicePackage
            {
                BranchId = dto.BranchId,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StatusCode = dto.StatusCode
            };

            // attach components if provided
            if (dto.ComponentIds != null && dto.ComponentIds.Any())
            {
                var comps = await _context.Components.Where(c => dto.ComponentIds.Contains(c.Id)).ToListAsync();
                entity.Components = comps;
            }

            var created = await _repo.AddAsync(entity);
            return MapToResponse(created);
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            await _repo.DisableEnableAsync(id, statusCode);
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(long? branchId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(branchId, statusCode, search);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return MapToResponse(e);
        }

        public async Task<ResponseDto?> UpdateAsync(RequestDto dto)
        {
            if (!dto.Id.HasValue) return null;
            var exist = await _repo.GetByIdAsync(dto.Id.Value);
            if (exist == null) return null;

            exist.BranchId = dto.BranchId;
            exist.Code = dto.Code;
            exist.Name = dto.Name;
            exist.Description = dto.Description;
            exist.Price = dto.Price;
            exist.StatusCode = dto.StatusCode;

            // update components relationship
            if (dto.ComponentIds != null)
            {
                // load components by ids
                var comps = await _context.Components.Where(c => dto.ComponentIds.Contains(c.Id)).ToListAsync();
                // replace collection
                exist.Components.Clear();
                foreach (var c in comps)
                    exist.Components.Add(c);
            }

            var updated = await _repo.UpdateAsync(exist);
            return MapToResponse(updated);
        }

        private ResponseDto MapToResponse(ServicePackage sp)
        {
            var dto = _mapper.Map<ResponseDto>(sp);
            dto.Components = sp.Components?.Select(c => new ResponseDto.ComponentSummary
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                UnitPrice = c.UnitPrice
            }).ToList();
            return dto;
        }
    }
}


