using AutoMapper;
using BE.vn.fpt.edu.DTOs.Branch;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;

namespace BE.vn.fpt.edu.services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public BranchService(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BranchResponseDto>> GetAllAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Select(b => _mapper.Map<BranchResponseDto>(b));
        }

        public async Task<BranchResponseDto?> GetByIdAsync(long id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return null;
            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<BranchResponseDto> CreateAsync(BranchRequestDto dto)
        {
            var branch = _mapper.Map<Branch>(dto);
            await _branchRepository.AddAsync(branch);
            await _branchRepository.SaveChangesAsync();
            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<BranchResponseDto?> UpdateAsync(long id, BranchRequestDto dto)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return null;

            branch.Name = dto.Name;
            branch.Phone = dto.Phone;
            branch.Address = dto.Address;
            branch.LaborRate = dto.LaborRate;

            await _branchRepository.UpdateAsync(branch);
            await _branchRepository.SaveChangesAsync();

            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return false;

            // Kiểm tra xem chi nhánh có đang được sử dụng không
            var isInUse = await _branchRepository.IsBranchInUseAsync(id);
            if (isInUse)
            {
                throw new InvalidOperationException("Không thể xóa chi nhánh này vì đang được sử dụng trong hệ thống");
            }

            await _branchRepository.DeleteAsync(branch);
            await _branchRepository.SaveChangesAsync();
            return true;
        }
    }
}

