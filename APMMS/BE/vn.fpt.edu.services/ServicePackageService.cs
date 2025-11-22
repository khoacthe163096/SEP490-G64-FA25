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
            // Validation: Package phải có ít nhất 1 component
            var hasComponents = (dto.Components != null && dto.Components.Any()) || 
                                (dto.ComponentIds != null && dto.ComponentIds.Any());
            if (!hasComponents)
            {
                throw new ArgumentException("ServicePackage phải có ít nhất 1 component");
            }

            var entity = new ServicePackage
            {
                BranchId = dto.BranchId,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StatusCode = dto.StatusCode
            };

            var created = await _repo.AddAsync(entity);
            
            // attach components if provided (sau khi đã có ID)
            if (dto.Components != null && dto.Components.Any())
            {
                // Validation: Components phải tồn tại
                var componentIds = dto.Components.Select(c => c.ComponentId).ToList();
                var existingComponents = await _context.Components
                    .Where(c => componentIds.Contains(c.Id))
                    .ToListAsync();
                
                if (existingComponents.Count != componentIds.Count)
                {
                    var foundIds = existingComponents.Select(c => c.Id).ToList();
                    var notFoundIds = componentIds.Except(foundIds).ToList();
                    throw new ArgumentException($"Không tìm thấy components với IDs: {string.Join(", ", notFoundIds)}");
                }
                
                // Validation: Components phải cùng branch với package
                if (created.BranchId.HasValue)
                {
                    var invalidComponents = existingComponents.Where(c => c.BranchId != created.BranchId.Value).ToList();
                    if (invalidComponents.Any())
                    {
                        throw new ArgumentException($"Các components {string.Join(", ", invalidComponents.Select(c => c.Id))} không thuộc cùng chi nhánh với gói dịch vụ này");
                    }
                }
                
                // Validation: Price >= 0 (nếu có)
                if (dto.Price.HasValue && dto.Price.Value < 0)
                {
                    throw new ArgumentException("Giá gói dịch vụ phải lớn hơn hoặc bằng 0");
                }
                
                foreach (var comp in dto.Components)
                {
                    // Validation: Quantity phải > 0
                    if (comp.Quantity <= 0)
                    {
                        throw new ArgumentException($"Quantity của component {comp.ComponentId} phải lớn hơn 0");
                    }
                    
                    created.ComponentPackages.Add(new ComponentPackage
                    {
                        ComponentId = comp.ComponentId,
                        ServicePackageId = created.Id,
                        Quantity = comp.Quantity
                    });
                }
                await _context.SaveChangesAsync();
            }
            // Backward compatibility: Nếu dùng ComponentIds (cũ)
            else if (dto.ComponentIds != null && dto.ComponentIds.Any())
            {
                foreach (var componentId in dto.ComponentIds)
                {
                    created.ComponentPackages.Add(new ComponentPackage
                    {
                        ComponentId = componentId,
                        ServicePackageId = created.Id,
                        Quantity = 1 // Default quantity
                    });
                }
                await _context.SaveChangesAsync();
            }
            
            return MapToResponse(created);
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            await _repo.DisableEnableAsync(id, statusCode);
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10, long? branchId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(page, pageSize, branchId, statusCode, search);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<int> GetTotalCountAsync(long? branchId = null, string? statusCode = null, string? search = null)
        {
            return await _repo.GetTotalCountAsync(branchId, statusCode, search);
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

            // Validation: Price >= 0 (nếu có)
            if (dto.Price.HasValue && dto.Price.Value < 0)
            {
                throw new ArgumentException("Giá gói dịch vụ phải lớn hơn hoặc bằng 0");
            }
            
            // update components relationship
            if (dto.Components != null && dto.Components.Any())
            {
                // Validation: Components phải tồn tại
                var componentIds = dto.Components.Select(c => c.ComponentId).ToList();
                var existingComponents = await _context.Components
                    .Where(c => componentIds.Contains(c.Id))
                    .ToListAsync();
                
                if (existingComponents.Count != componentIds.Count)
                {
                    var foundIds = existingComponents.Select(c => c.Id).ToList();
                    var notFoundIds = componentIds.Except(foundIds).ToList();
                    throw new ArgumentException($"Không tìm thấy components với IDs: {string.Join(", ", notFoundIds)}");
                }
                
                // Validation: Components phải cùng branch với package
                if (exist.BranchId.HasValue)
                {
                    var invalidComponents = existingComponents.Where(c => c.BranchId != exist.BranchId.Value).ToList();
                    if (invalidComponents.Any())
                    {
                        throw new ArgumentException($"Các components {string.Join(", ", invalidComponents.Select(c => c.Id))} không thuộc cùng chi nhánh với gói dịch vụ này");
                    }
                }
                
                // Validation: Quantity phải > 0 cho mỗi component
                foreach (var comp in dto.Components)
                {
                    if (comp.Quantity <= 0)
                    {
                        throw new ArgumentException($"Quantity của component {comp.ComponentId} phải lớn hơn 0");
                    }
                }
                
                // Remove existing ComponentPackages
                var existingComponentPackages = await _context.ComponentPackages
                    .Where(cp => cp.ServicePackageId == exist.Id)
                    .ToListAsync();
                _context.ComponentPackages.RemoveRange(existingComponentPackages);
                
                // Add new ComponentPackages với Quantity từ DTO
                foreach (var comp in dto.Components)
                {
                    exist.ComponentPackages.Add(new ComponentPackage
                    {
                        ComponentId = comp.ComponentId,
                        ServicePackageId = exist.Id,
                        Quantity = comp.Quantity
                    });
                }
            }
            // Backward compatibility: Nếu dùng ComponentIds (cũ)
            else if (dto.ComponentIds != null && dto.ComponentIds.Any())
            {
                // Remove existing ComponentPackages
                var existingComponentPackages = await _context.ComponentPackages
                    .Where(cp => cp.ServicePackageId == exist.Id)
                    .ToListAsync();
                _context.ComponentPackages.RemoveRange(existingComponentPackages);
                
                // Add new ComponentPackages với Quantity mặc định = 1
                foreach (var componentId in dto.ComponentIds)
                {
                    exist.ComponentPackages.Add(new ComponentPackage
                    {
                        ComponentId = componentId,
                        ServicePackageId = exist.Id,
                        Quantity = 1 // Default quantity
                    });
                }
            }

            var updated = await _repo.UpdateAsync(exist);
            return MapToResponse(updated);
        }

        private ResponseDto MapToResponse(ServicePackage sp)
        {
            var dto = _mapper.Map<ResponseDto>(sp);
            dto.BranchName = sp.Branch?.Name;
            dto.Components = sp.ComponentPackages?.Select(cp => new ResponseDto.ComponentSummary
            {
                Id = cp.Component.Id,
                Code = cp.Component.Code,
                Name = cp.Component.Name,
                UnitPrice = cp.Component.UnitPrice,
                QuantityStock = cp.Component.QuantityStock,
                ImageUrl = cp.Component.ImageUrl,
                Quantity = cp.Quantity // Lấy từ bảng trung gian
            }).ToList();
            
            // ❌ ĐÃ LOẠI BỎ: Map ServiceCategories
            // Lý do: ServiceCategory chỉ dùng cho ScheduleService (đặt lịch), không phải để tạo công việc thực tế
            // ServicePackage chỉ nên chứa Components (phụ tùng)
            // ServiceTasks nên được tạo thủ công bởi người dùng
            dto.ServiceCategories = null; // Không load ServiceCategories
            
            return dto;
        }
    }
}


