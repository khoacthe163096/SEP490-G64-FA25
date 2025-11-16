using AutoMapper;
using BE.vn.fpt.edu.DTOs.CarOfAutoOwner;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

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
            var result = _mapper.Map<List<ResponseDto>>(cars);
            
            // Đảm bảo VehicleTypeName được set đúng
            for (int i = 0; i < result.Count; i++)
            {
                if (string.IsNullOrEmpty(result[i].VehicleTypeName) && cars[i].VehicleType != null)
                {
                    result[i].VehicleTypeName = cars[i].VehicleType.Name;
                }
            }
            
            return result;
        }

        public async Task<List<ResponseDto>> GetServicedCarsByUserIdAsync(long userId)
        {
            var cars = await _repository.GetServicedCarsByUserIdAsync(userId);
            return _mapper.Map<List<ResponseDto>>(cars);
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Request DTO cannot be null.");
            }

            if (!dto.UserId.HasValue || dto.UserId.Value <= 0)
            {
                throw new ArgumentException("UserId is required and must be greater than 0.", nameof(dto));
            }

            var car = _mapper.Map<Car>(dto);
            car.CreatedDate = DateTime.UtcNow;
            // Xe của khách hàng không cần chi nhánh
            car.BranchId = null;

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
            // Xe của khách hàng không cần chi nhánh
            existing.BranchId = null;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
