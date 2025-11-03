using AutoMapper;
using BE.models;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.interfaces;
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

        public async Task<IEnumerable<ResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseDto>>(list);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseDto>(entity);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = _mapper.Map<TypeComponent>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);

            return _mapper.Map<ResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}