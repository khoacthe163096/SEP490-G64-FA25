using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.StockIn;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class StockInService : IStockInService
    {
        private readonly IStockInRepository _repo;
        private readonly IStockInRequestRepository _requestRepo;
        private readonly IComponentRepository _componentRepo;
        private readonly IMapper _mapper;
        private readonly CarMaintenanceDbContext _context;

        public StockInService(
            IStockInRepository repo,
            IStockInRequestRepository requestRepo,
            IComponentRepository componentRepo,
            IMapper mapper,
            CarMaintenanceDbContext context)
        {
            _repo = repo;
            _requestRepo = requestRepo;
            _componentRepo = componentRepo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<StockInResponseDto> CreateFromRequestAsync(StockInRequestDto dto, long userId)
        {
            // Validate stock in request exists and is approved
            StockInRequest? request = null;
            if (dto.StockInRequestId.HasValue)
            {
                request = await _requestRepo.GetByIdAsync(dto.StockInRequestId.Value);
            }
            else if (!string.IsNullOrEmpty(dto.StockInRequestCode))
            {
                request = await _requestRepo.GetByCodeAsync(dto.StockInRequestCode);
            }

            if (request == null)
                throw new ArgumentException("Yêu cầu nhập kho không tồn tại");

            if (request.StatusCode != "APPROVED")
                throw new ArgumentException("Chỉ có thể tạo phiếu nhập kho từ yêu cầu đã được duyệt (APPROVED)");

            // Check if stock in already exists for this request
            if (await _repo.ExistsByStockInRequestIdAsync(request.Id))
                throw new ArgumentException("Phiếu nhập kho đã tồn tại cho yêu cầu này");

            // Validate details
            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("Phiếu nhập kho phải có ít nhất một linh kiện");

            // Create stock in entity
            var entity = new StockIn
            {
                StockInRequestId = request.Id,
                StatusCode = dto.StatusCode ?? "CREATED",
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            // Create details with snapshot
            entity.StockInDetails = new List<StockInDetail>();
            foreach (var detailDto in dto.Details)
            {
                var component = await _componentRepo.GetByIdAsync(detailDto.ComponentId ?? 0);
                if (component == null && !string.IsNullOrEmpty(detailDto.ComponentCode))
                {
                    // Try to find by code
                    var components = await _componentRepo.GetAllAsync(1, 1000);
                    component = components.FirstOrDefault(c => c.Code == detailDto.ComponentCode);
                }

                var detail = new StockInDetail
                {
                    StockInId = entity.Id, // Will be set after save
                    ComponentId = component?.Id ?? detailDto.ComponentId ?? 0,
                    Quantity = detailDto.Quantity,
                    QuantityAfterCheck = detailDto.QuantityAfterCheck ?? detailDto.Quantity,
                    ImportPricePerUnit = detailDto.ImportPricePerUnit,
                    ExportPricePerUnit = detailDto.ExportPricePerUnit,
                    Vat = detailDto.Vat,
                    ComponentCode = detailDto.ComponentCode ?? component?.Code ?? "",
                    ComponentName = detailDto.ComponentName,
                    TypeComponent = detailDto.TypeComponent ?? component?.TypeComponent?.Name,
                    MinQuantity = detailDto.MinQuantity ?? component?.MinimumQuantity,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                entity.StockInDetails.Add(detail);
            }

            var created = await _repo.AddAsync(entity);
            return await MapToResponseAsync(created);
        }

        public async Task<StockInResponseDto> UpdateAsync(StockInRequestDto dto, long userId)
        {
            if (!dto.Id.HasValue)
                throw new ArgumentException("ID phiếu nhập kho là bắt buộc");

            var entity = await _repo.GetByIdAsync(dto.Id.Value);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");

            // Only allow update if status is CREATED or CANCELLED
            if (entity.StatusCode != "CREATED" && entity.StatusCode != "CANCELLED")
                throw new ArgumentException("Không thể cập nhật phiếu nhập kho ở trạng thái này");

            entity.StatusCode = dto.StatusCode ?? entity.StatusCode;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            // Update details (quantity after check, prices)
            if (dto.Details != null && dto.Details.Any())
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = entity.StockInDetails?.FirstOrDefault(d => 
                        d.ComponentCode == detailDto.ComponentCode || 
                        d.ComponentId == (detailDto.ComponentId ?? 0));
                    
                    if (detail != null)
                    {
                        if (detailDto.QuantityAfterCheck.HasValue)
                            detail.QuantityAfterCheck = detailDto.QuantityAfterCheck.Value;
                        if (detailDto.ImportPricePerUnit.HasValue)
                            detail.ImportPricePerUnit = detailDto.ImportPricePerUnit;
                        if (detailDto.ExportPricePerUnit.HasValue)
                            detail.ExportPricePerUnit = detailDto.ExportPricePerUnit;
                        if (detailDto.Vat.HasValue)
                            detail.Vat = detailDto.Vat;
                        detail.LastModifiedBy = userId;
                        detail.LastModifiedDate = DateTime.Now;
                    }
                }
            }

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInResponseDto> GetByIdAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");
            return await MapToResponseAsync(entity);
        }

        public async Task<IEnumerable<StockInResponseDto>> GetAllAsync(int page = 1, int pageSize = 10, long? branchId = null, string? statusCode = null, string? search = null)
        {
            var list = await _repo.GetAllAsync(page, pageSize, branchId, statusCode, search);
            var result = new List<StockInResponseDto>();
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

        public async Task<IEnumerable<StockInResponseDto>> GetByStatusAsync(string statusCode)
        {
            var list = await _repo.GetByStatusAsync(statusCode);
            var result = new List<StockInResponseDto>();
            foreach (var entity in list)
            {
                result.Add(await MapToResponseAsync(entity));
            }
            return result;
        }

        public async Task<StockInResponseDto> ChangeStatusAsync(long id, string statusCode, long userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");

            // Validate status transition
            if (statusCode == "PENDING")
            {
                if (entity.StatusCode != "CREATED" && entity.StatusCode != "CANCELLED")
                    throw new ArgumentException("Chỉ có thể gửi phiếu từ trạng thái CREATED hoặc CANCELLED");
            }

            entity.StatusCode = statusCode;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInResponseDto> ApproveAsync(StockInRequestDto dto, long userId)
        {
            if (!dto.Id.HasValue)
                throw new ArgumentException("ID phiếu nhập kho là bắt buộc");

            var entity = await _repo.GetByIdAsync(dto.Id.Value);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");

            if (entity.StatusCode != "PENDING")
                throw new ArgumentException("Chỉ có thể duyệt phiếu ở trạng thái PENDING");

            // Validate prices and quantities
            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("Phiếu nhập kho phải có ít nhất một linh kiện");

            foreach (var detailDto in dto.Details)
            {
                if (!detailDto.QuantityAfterCheck.HasValue || detailDto.QuantityAfterCheck.Value <= 0)
                    throw new ArgumentException("Số lượng sau kiểm tra phải lớn hơn 0");

                if (!detailDto.ImportPricePerUnit.HasValue || detailDto.ImportPricePerUnit.Value <= 0)
                    throw new ArgumentException("Giá nhập phải lớn hơn 0");

                if (!detailDto.ExportPricePerUnit.HasValue || detailDto.ExportPricePerUnit.Value <= 0)
                    throw new ArgumentException("Giá xuất phải lớn hơn 0");

                if (!detailDto.Vat.HasValue || detailDto.Vat.Value < 0)
                    throw new ArgumentException("VAT phải >= 0");
            }

            // Update details with prices and quantities
            foreach (var detailDto in dto.Details)
            {
                var detail = entity.StockInDetails?.FirstOrDefault(d => 
                    d.ComponentCode == detailDto.ComponentCode || 
                    d.ComponentId == (detailDto.ComponentId ?? 0));
                
                if (detail != null)
                {
                    detail.QuantityAfterCheck = detailDto.QuantityAfterCheck.Value;
                    detail.ImportPricePerUnit = detailDto.ImportPricePerUnit.Value;
                    detail.ExportPricePerUnit = detailDto.ExportPricePerUnit.Value;
                    detail.Vat = detailDto.Vat ?? 0;
                    detail.LastModifiedBy = userId;
                    detail.LastModifiedDate = DateTime.Now;
                }
            }

            // Update stock in component quantities
            foreach (var detail in entity.StockInDetails)
            {
                if (detail.ComponentId > 0)
                {
                    var component = await _componentRepo.GetByIdAsync(detail.ComponentId);
                    if (component != null)
                    {
                        // Update component stock
                        component.QuantityStock = (component.QuantityStock ?? 0) + (detail.QuantityAfterCheck ?? 0);
                        
                        // Update prices if provided
                        if (detail.ImportPricePerUnit.HasValue)
                            component.PurchasePrice = detail.ImportPricePerUnit.Value;
                        if (detail.ExportPricePerUnit.HasValue)
                            component.UnitPrice = detail.ExportPricePerUnit.Value;
                        // VAT chỉ lưu trong StockInDetail, không lưu trong Component

                        await _componentRepo.UpdateAsync(component);
                    }
                }
            }

            // Update stock in status
            entity.StatusCode = "APPROVED";
            entity.ApprovedBy = userId;
            entity.ApprovedAt = DateTime.Now;
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInResponseDto> UpdateQuantityAfterCheckAsync(StockInRequestDto dto, long userId)
        {
            if (!dto.Id.HasValue)
                throw new ArgumentException("ID phiếu nhập kho là bắt buộc");

            var entity = await _repo.GetByIdAsync(dto.Id.Value);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");

            // Only allow update if status is CREATED or CANCELLED
            if (entity.StatusCode != "CREATED" && entity.StatusCode != "CANCELLED")
                throw new ArgumentException("Không thể cập nhật số lượng sau kiểm tra ở trạng thái này");

            if (dto.Details == null || !dto.Details.Any())
                throw new ArgumentException("Phải có ít nhất một linh kiện");

            // Update quantity after check
            foreach (var detailDto in dto.Details)
            {
                var detail = entity.StockInDetails?.FirstOrDefault(d => 
                    d.ComponentCode == detailDto.ComponentCode || 
                    d.ComponentId == (detailDto.ComponentId ?? 0));
                
                if (detail != null && detailDto.QuantityAfterCheck.HasValue)
                {
                    detail.QuantityAfterCheck = detailDto.QuantityAfterCheck.Value;
                    detail.LastModifiedBy = userId;
                    detail.LastModifiedDate = DateTime.Now;
                }
            }

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<StockInResponseDto> CancelAsync(long id, long userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new ArgumentException("Phiếu nhập kho không tồn tại");

            if (entity.StatusCode == "APPROVED")
                throw new ArgumentException("Không thể hủy phiếu đã được duyệt");

            entity.StatusCode = "CANCELLED";
            entity.LastModifiedBy = userId;
            entity.LastModifiedDate = DateTime.Now;

            var updated = await _repo.UpdateAsync(entity);
            return await MapToResponseAsync(updated);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _repo.ExistsAsync(id);
        }

        private async Task<StockInResponseDto> MapToResponseAsync(StockIn entity)
        {
            var dto = _mapper.Map<StockInResponseDto>(entity);
            dto.StockInRequestCode = entity.StockInRequest?.Code ?? "";
            dto.StatusName = entity.StatusCodeNavigation?.Name;
            dto.CreatedByName = entity.CreatedByNavigation != null
                ? $"{entity.CreatedByNavigation.FirstName} {entity.CreatedByNavigation.LastName}".Trim()
                : null;
            dto.ApprovedByName = entity.ApprovedByNavigation != null
                ? $"{entity.ApprovedByNavigation.FirstName} {entity.ApprovedByNavigation.LastName}".Trim()
                : null;
            dto.LastModifiedByName = entity.LastModifiedByNavigation != null
                ? $"{entity.LastModifiedByNavigation.FirstName} {entity.LastModifiedByNavigation.LastName}".Trim()
                : null;

            // Map details
            if (entity.StockInDetails != null && entity.StockInDetails.Any())
            {
                dto.Details = entity.StockInDetails.Select(d => new StockInDetailResponseDto
                {
                    Id = d.Id,
                    StockInId = d.StockInId,
                    ComponentId = d.ComponentId,
                    ComponentCode = d.ComponentCode ?? "",
                    ComponentName = d.ComponentName ?? "",
                    TypeComponent = d.TypeComponent,
                    Quantity = d.Quantity,
                    QuantityAfterCheck = d.QuantityAfterCheck,
                    ImportPricePerUnit = d.ImportPricePerUnit,
                    ExportPricePerUnit = d.ExportPricePerUnit,
                    Vat = d.Vat,
                    MinQuantity = d.MinQuantity,
                    CreatedAt = d.CreatedAt,
                    LastModifiedDate = d.LastModifiedDate
                }).ToList();
            }

            return dto;
        }
    }
}

