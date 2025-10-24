using AutoMapper;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class TypeComponentService : ITypeComponentService
    {
        private readonly ITypeComponentRepository _typeComponentRepository;
        private readonly IMapper _mapper;

        public TypeComponentService(ITypeComponentRepository typeComponentRepository, IMapper mapper)
        {
            _typeComponentRepository = typeComponentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TypeComponentResponseDto>> GetAllTypeComponentsAsync()
        {
            var typeComponents = await _typeComponentRepository.GetAllAsync();
            return typeComponents.Select(tc => new TypeComponentResponseDto
            {
                Id = tc.Id,
                Name = tc.Name,
                Description = tc.Description,
                ComponentCount = tc.Components.Count
            });
        }

        public async Task<TypeComponentDetailResponseDto?> GetTypeComponentByIdAsync(long id)
        {
            var typeComponent = await _typeComponentRepository.GetByIdWithComponentsAsync(id);
            if (typeComponent == null) return null;

            return new TypeComponentDetailResponseDto
            {
                Id = typeComponent.Id,
                Name = typeComponent.Name,
                Description = typeComponent.Description,
                Components = typeComponent.Components.Select(c => new ComponentInfoDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    UnitPrice = c.UnitPrice,
                    QuantityStock = c.QuantityStock,
                    ImageUrl = c.ImageUrl
                }).ToList()
            };
        }

        public async Task<TypeComponentResponseDto> CreateTypeComponentAsync(CreateTypeComponentDto request)
        {
            var typeComponent = new TypeComponent
            {
                Name = request.Name,
                Description = request.Description
            };

            await _typeComponentRepository.AddAsync(typeComponent);
            await _typeComponentRepository.SaveChangesAsync();

            return new TypeComponentResponseDto
            {
                Id = typeComponent.Id,
                Name = typeComponent.Name,
                Description = typeComponent.Description,
                ComponentCount = 0
            };
        }

        public async Task<TypeComponentResponseDto> UpdateTypeComponentAsync(long id, UpdateTypeComponentDto request)
        {
            var typeComponent = await _typeComponentRepository.GetByIdWithComponentsAsync(id);
            if (typeComponent == null)
                throw new ArgumentException("Type component not found");

            typeComponent.Name = request.Name;
            typeComponent.Description = request.Description;

            await _typeComponentRepository.UpdateAsync(typeComponent);
            await _typeComponentRepository.SaveChangesAsync();

            return new TypeComponentResponseDto
            {
                Id = typeComponent.Id,
                Name = typeComponent.Name,
                Description = typeComponent.Description,
                ComponentCount = typeComponent.Components.Count
            };
        }

        public async Task<bool> DeleteTypeComponentAsync(long id)
        {
            var typeComponent = await _typeComponentRepository.GetByIdAsync(id);
            if (typeComponent == null) return false;

            await _typeComponentRepository.DeleteAsync(typeComponent);
            await _typeComponentRepository.SaveChangesAsync();
            return true;
        }
    }
}