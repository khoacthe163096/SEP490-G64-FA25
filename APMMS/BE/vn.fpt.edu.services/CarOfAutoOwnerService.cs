using AutoMapper;
using BE.vn.fpt.edu.DTOs.CarOfAutoOwner;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class CarOfAutoOwnerService : ICarOfAutoOwnerService
    {
        private readonly ICarOfAutoOwnerRepository _repository;
        private readonly IMapper _mapper;

        public CarOfAutoOwnerService(ICarOfAutoOwnerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var cars = await _repository.GetAllAsync(page, pageSize);
            return _mapper.Map<List<ResponseDto>>(cars);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var car = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResponseDto?>(car);
        }

        public async Task<List<ResponseDto>> GetByUserIdAsync(long userId)
        {
            var cars = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<List<ResponseDto>>(cars);
        }

        public async Task<List<ResponseDto>> GetServicedCarsByUserIdAsync(long userId)
        {
            var cars = await _repository.GetServicedCarsByUserIdAsync(userId);
            return _mapper.Map<List<ResponseDto>>(cars);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            var car = _mapper.Map<Car>(dto);
            car.CreatedDate = DateTime.UtcNow;

            await _repository.CreateAsync(car);
            return _mapper.Map<ResponseDto>(car);
        }

        public async Task<ResponseDto> UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Car not found.");

            _mapper.Map(dto, existing);
            existing.LastModifiedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
