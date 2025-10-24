using AutoMapper;
using BE.vn.fpt.edu.DTOs.Component;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class ComponentService : IComponentService
    {
        private readonly IComponentRepository _componentRepository;
        private readonly IMapper _mapper;

        public ComponentService(IComponentRepository componentRepository, IMapper mapper)
        {
            _componentRepository = componentRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<ComponentResponseDto> components, PaginationDto pagination)> GetAllComponentsAsync(int page = 1, int pageSize = 10, long? typeComponentId = null, long? branchId = null)
        {
            IEnumerable<Component> components;
            int totalCount;

            if (typeComponentId.HasValue)
            {
                components = await _componentRepository.GetByTypeComponentIdAsync(typeComponentId.Value, page, pageSize);
                totalCount = await _componentRepository.GetCountByTypeComponentIdAsync(typeComponentId.Value);
            }
            else if (branchId.HasValue)
            {
                components = await _componentRepository.GetByBranchIdAsync(branchId.Value, page, pageSize);
                totalCount = await _componentRepository.GetCountByBranchIdAsync(branchId.Value);
            }
            else
            {
                components = await _componentRepository.GetAllWithDetailsAsync(page, pageSize);
                totalCount = await _componentRepository.GetTotalCountAsync();
            }

            var componentDtos = components.Select(MapToResponseDto);
            var pagination = new PaginationDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return (componentDtos, pagination);
        }

        public async Task<ComponentResponseDto?> GetComponentByIdAsync(long id)
        {
            var component = await _componentRepository.GetByIdWithDetailsAsync(id);
            if (component == null) return null;

            return MapToResponseDto(component);
        }

        public async Task<(IEnumerable<ComponentResponseDto> components, PaginationDto pagination)> GetComponentsByTypeComponentIdAsync(long typeComponentId, int page = 1, int pageSize = 10)
        {
            var components = await _componentRepository.GetByTypeComponentIdAsync(typeComponentId, page, pageSize);
            var totalCount = await _componentRepository.GetCountByTypeComponentIdAsync(typeComponentId);

            var componentDtos = components.Select(MapToResponseDto);
            var pagination = new PaginationDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return (componentDtos, pagination);
        }

        public async Task<ComponentResponseDto> CreateComponentAsync(CreateComponentDto request)
        {
            var component = new Component
            {
                Name = request.Name,
                Code = request.Code,
                UnitPrice = request.UnitPrice,
                QuantityStock = request.QuantityStock,
                TypeComponentId = request.TypeComponentId,
                BranchId = request.BranchId,
                ImageUrl = request.ImageUrl
            };

            await _componentRepository.AddAsync(component);
            await _componentRepository.SaveChangesAsync();

            var createdComponent = await _componentRepository.GetByIdWithDetailsAsync(component.Id);
            return MapToResponseDto(createdComponent!);
        }

        public async Task<ComponentResponseDto> UpdateComponentAsync(long id, UpdateComponentDto request)
        {
            var component = await _componentRepository.GetByIdAsync(id);
            if (component == null)
                throw new ArgumentException("Component not found");

            component.Name = request.Name;
            component.Code = request.Code;
            component.UnitPrice = request.UnitPrice;
            component.QuantityStock = request.QuantityStock;
            component.TypeComponentId = request.TypeComponentId;
            component.BranchId = request.BranchId;
            component.ImageUrl = request.ImageUrl;

            await _componentRepository.UpdateAsync(component);
            await _componentRepository.SaveChangesAsync();

            var updatedComponent = await _componentRepository.GetByIdWithDetailsAsync(id);
            return MapToResponseDto(updatedComponent!);
        }

        public async Task<bool> DeleteComponentAsync(long id)
        {
            var component = await _componentRepository.GetByIdAsync(id);
            if (component == null) return false;

            await _componentRepository.DeleteAsync(component);
            await _componentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ComponentResponseDto> UpdateStockAsync(long id, UpdateStockDto request)
        {
            var component = await _componentRepository.GetByIdAsync(id);
            if (component == null)
                throw new ArgumentException("Component not found");

            component.QuantityStock = request.QuantityStock;
            await _componentRepository.UpdateAsync(component);
            await _componentRepository.SaveChangesAsync();

            var updatedComponent = await _componentRepository.GetByIdWithDetailsAsync(id);
            return MapToResponseDto(updatedComponent!);
        }

        private ComponentResponseDto MapToResponseDto(Component component)
        {
            return new ComponentResponseDto
            {
                Id = component.Id,
                Name = component.Name,
                Code = component.Code,
                UnitPrice = component.UnitPrice,
                QuantityStock = component.QuantityStock,
                ImageUrl = component.ImageUrl,
                TypeComponentId = component.TypeComponentId,
                TypeComponentName = component.TypeComponent?.Name,
                BranchId = component.BranchId,
                BranchName = component.Branch?.Name
            };
        }
    }
}