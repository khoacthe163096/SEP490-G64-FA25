using AutoMapper;
using BE.models;
using BE.vn.fpt.edu.DTOs.ServicePackage;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class ServicePackageService : IServicePackageService
    {
        private readonly IServicePackageRepository _repo;
        private readonly IMapper _mapper;

        public ServicePackageService(IServicePackageRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseDto>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseDto>>(data);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var data = await _repo.GetByIdAsync(id);
            return _mapper.Map<ResponseDto?>(data);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var entity = _mapper.Map<ServicePackage>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ResponseDto>(entity);
        }

        public async Task<bool> UpdateAsync(long id, RequestDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
