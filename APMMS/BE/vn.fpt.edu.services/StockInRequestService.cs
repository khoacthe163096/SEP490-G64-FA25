using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.StockInRequest;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class StockInRequestService : IStockInRequestService
    {
        private readonly IStockInRequestRepository _repo;
        private readonly IComponentRepository _componentRepo;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public StockInRequestService(
            IStockInRequestRepository repo,
            IComponentRepository componentRepo,
            IMapper mapper,
            CarMaintenanceDbContext context)
        {
            _repo = repo;
            _componentRepo = componentRepo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<StockInRequestResponseDto> CreateAsync(StockInRequestRequestDto dto, long userId)
        {
            // Validate userId
            if (userId <= 0)
            {
                throw new ArgumentException("UserId không hợp lệ. Vui lòng đăng nhập lại.");
            }

            System.Diagnostics.Debug.WriteLine($"[StockInRequestService] Creating StockInRequest with UserId: {userId}, BranchId: {dto.BranchId}");

            // Validation
            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("Yêu cầu nhập kho phải có ít nhất một linh kiện");

            // Generate code if not provided
            if (string.IsNullOrEmpty(dto.Code))
            {
                dto.Code = await GenerateUniqueCodeAsync();
            }
            else if (await _repo.CodeExistsAsync(dto.Code))
            {
                throw new ArgumentException($"Mã yêu cầu nhập kho '{dto.Code}' đã tồn tại");
            }

            // Validate BranchId
            if (dto.BranchId <= 0)
            {
                throw new ArgumentException("BranchId không hợp lệ. Vui lòng liên hệ quản trị viên.");
            }

            // Create entity - Status mặc định là CREATED (chưa gửi)
            var entity = new StockInRequest
            {
                Code = dto.Code,
                BranchId = dto.BranchId,
                Description = dto.Description,
                Note = dto.Note,
                StatusCode = dto.StatusCode ?? "CREATED", // Mặc định là CREATED, phải gửi đơn mới thành PENDING
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            System.Diagnostics.Debug.WriteLine($"[StockInRequestService] Created entity: Code={entity.Code}, BranchId={entity.BranchId}, CreatedBy={entity.CreatedBy}");

            // Add details
            entity.StockInRequestDetails = new List<StockInRequestDetail>();
            foreach (var detailDto in dto.Details)
            {
                // Validate component exists
                var component = await _componentRepo.GetByIdAsync(detailDto.ComponentId);
                if (component == null)
                    throw new ArgumentException($"Linh kiện với ID {detailDto.ComponentId} không tồn tại");

                // Validate component belongs to same branch
                System.Diagnostics.Debug.WriteLine($"[StockInRequestService] Validating component: {component.Name} (ID: {component.Id}), Component.BranchId: {component.BranchId}, Request.BranchId: {dto.BranchId}");

                if (!component.BranchId.HasValue)
                    throw new ArgumentException($"Linh kiện {component.Name} chưa được gán cho chi nhánh nào");

                if (component.BranchId.Value != dto.BranchId)
                {
                    var componentBranch = component.Branch?.Name ?? "N/A";
                    throw new ArgumentException($"Linh kiện {component.Name} (Mã: {component.Code ?? "N/A"}) không thuộc chi nhánh này. Linh kiện thuộc chi nhánh: {componentBranch}.");
                }

                entity.StockInRequestDetails.Add(new StockInRequestDetail
                {
                    StockInRequestId = entity.Id, // Will be set after save
                    ComponentId = detailDto.ComponentId,
                    Quantity = detailDto.Quantity
                });
            }

            var created = await _repo.AddAsync(entity);
            return await MapToResponseAsync(created);
        }

        public async Task<StockInRequestResponseDto> UpdateAsync(StockInRequestRequestDto dto, long userId)
        {
            if (!dto.StockInRequestId.HasValue)
                throw new ArgumentException("ID yêu cầu nhập kho là bắt buộc");

            var entity = await _repo.GetByIdAsync(dto.StockInRequestId.Value);
            if (entity == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");

            // Only allow update if status is PENDING, CREATED, or CANCELLED
            if (entity.StatusCode != "PENDING" && entity.StatusCode != "CREATED" && entity.StatusCode != "CANCELLED")
                throw new ArgumentException("Không thể cập nhật yêu cầu nhập kho ở trạng thái này");

            // Update basic info
            entity.Description = dto.Description;
            entity.Note = dto.Note;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            // Update code if changed
            if (!string.IsNullOrEmpty(dto.Code) && dto.Code != entity.Code)
            {
                if (await _repo.CodeExistsAsync(dto.Code, dto.StockInRequestId))
                    throw new ArgumentException($"Mã yêu cầu nhập kho '{dto.Code}' đã tồn tại");
                entity.Code = dto.Code;
            }

            // Update details
            if (dto.Details != null && dto.Details.Any())
            {
                // Remove all existing details
                _context.StockInRequestDetails.RemoveRange(entity.StockInRequestDetails);

                // Add new details
                entity.StockInRequestDetails = new List<StockInRequestDetail>();
                foreach (var detailDto in dto.Details)
                {
                    if (detailDto.Quantity <= 0)
                        continue; // Skip if quantity is 0 (means delete)

                    var component = await _componentRepo.GetByIdAsync(detailDto.ComponentId);
                    if (component == null)
                        throw new ArgumentException($"Linh kiện với ID {detailDto.ComponentId} không tồn tại");

                    if (!component.BranchId.HasValue)
                        throw new ArgumentException($"Linh kiện {component.Name} chưa được gán cho chi nhánh nào");

                    if (component.BranchId.Value != entity.BranchId)
                    {
                        var componentBranch = component.Branch?.Name ?? "N/A";
                        throw new ArgumentException($"Linh kiện {component.Name} (Mã: {component.Code ?? "N/A"}) không thuộc chi nhánh này. Linh kiện thuộc chi nhánh: {componentBranch}.");
                    }

                    entity.StockInRequestDetails.Add(new StockInRequestDetail
                    {
                        StockInRequestId = entity.Id,
                        ComponentId = detailDto.ComponentId,
                        Quantity = detailDto.Quantity
                    });
                }

                if (!entity.StockInRequestDetails.Any())
                    throw new ArgumentException("Yêu cầu nhập kho phải có ít nhất một linh kiện");
            }

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInRequestResponseDto> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");
            return await MapToResponseAsync(entity);
        }

        public async Task<IEnumerable<StockInRequestResponseDto>> GetAllAsync(int page = 1, int pageSize = 10, long? branchId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(page, pageSize, branchId, statusCode, search);
            var result = new List<StockInRequestResponseDto>();
            foreach (var entity in list)
            {
                result.Add(await MapToResponseAsync(entity));
            }
            return result;
        }

        public async Task<int> GetTotalCountAsync(long? branchId = null, string? statusCode = null, string? search = null)
        {
            return await _repo.GetTotalCountAsync(branchId, statusCode, search);
        }

        public async Task<IEnumerable<StockInRequestResponseDto>> GetByStatusAsync(string statusCode)
        {
            var list = await _repo.GetByStatusAsync(statusCode);
            var result = new List<StockInRequestResponseDto>();
            foreach (var entity in list)
            {
                result.Add(await MapToResponseAsync(entity));
            }
            return result;
        }

        public async Task<StockInRequestResponseDto> ChangeStatusAsync(long id, string statusCode, long userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");

            // Validate status transition
            if (statusCode == "PENDING")
            {
                if (entity.StatusCode != "CREATED" && entity.StatusCode != "CANCELLED")
                    throw new ArgumentException("Chỉ có thể gửi yêu cầu từ trạng thái CREATED hoặc CANCELLED");
            }

            entity.StatusCode = statusCode;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInRequestResponseDto> ApproveAsync(long id, long userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");

            if (entity.StatusCode != "PENDING")
                throw new ArgumentException("Chỉ có thể duyệt yêu cầu ở trạng thái PENDING");

            entity.StatusCode = "APPROVED";
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInRequestResponseDto> CancelAsync(long id, string? note, long userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");

            if (entity.StatusCode == "APPROVED")
                throw new ArgumentException("Không thể hủy yêu cầu đã được duyệt");

            entity.StatusCode = "CANCELLED";
            if (!string.IsNullOrEmpty(note))
                entity.Note = note;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _repo.ExistsAsync(id);
        }

        private async Task<StockInRequestResponseDto> MapToResponseAsync(StockInRequest entity)
        {
            var dto = _mapper.Map<StockInRequestResponseDto>(entity);
            dto.BranchName = entity.Branch?.Name;
            dto.StatusName = entity.StatusCodeNavigation?.Name;
            dto.CreatedByName = entity.CreatedByNavigation != null 
                ? $"{entity.CreatedByNavigation.FirstName} {entity.CreatedByNavigation.LastName}".Trim()
                : null;
            dto.LastModifiedByName = entity.LastModifiedByNavigation != null
                ? $"{entity.LastModifiedByNavigation.FirstName} {entity.LastModifiedByNavigation.LastName}".Trim()
                : null;

            // Map details
            if (entity.StockInRequestDetails != null && entity.StockInRequestDetails.Any())
            {
                dto.Details = entity.StockInRequestDetails.Select(d => new StockInRequestDetailResponseDto
                {
                    StockInRequestId = d.StockInRequestId,
                    ComponentId = d.ComponentId,
                    ComponentCode = d.Component?.Code ?? "",
                    ComponentName = d.Component?.Name ?? "",
                    ImageUrl = d.Component?.ImageUrl,
                    Quantity = d.Quantity,
                    ImportPricePerUnit = d.Component?.PurchasePrice,
                    ExportPricePerUnit = d.Component?.UnitPrice,
                    MinQuantity = d.Component?.MinimumQuantity
                }).ToList();
            }

            return dto;
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            string prefix = "YCK";
            string code;
            int counter = 1;
            do
            {
                code = $"{prefix}{DateTime.Now:yyyyMMdd}{counter:D4}";
                counter++;
            } while (await _repo.CodeExistsAsync(code) && counter < 10000);

            if (counter >= 10000)
                throw new Exception("Không thể tạo mã yêu cầu nhập kho duy nhất");

            return code;
        }
    }
}

