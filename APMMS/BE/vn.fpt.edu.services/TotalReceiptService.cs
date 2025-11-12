using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BE.vn.fpt.edu.DTOs.TotalReceipt;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class TotalReceiptService : ITotalReceiptService
    {
        private readonly ITotalReceiptRepository _repository;
        private readonly CarMaintenanceDbContext _context;
        private readonly IMapper _mapper;

        public TotalReceiptService(ITotalReceiptRepository repository,
                                   CarMaintenanceDbContext context,
                                   IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
        {
            if (!dto.MaintenanceTicketId.HasValue)
            {
                throw new ArgumentException("MaintenanceTicketId is required");
            }

            var maintenanceTicket = await _context.MaintenanceTickets
                .Include(mt => mt.Car)
                    .ThenInclude(c => c.User)
                .Include(mt => mt.Branch)
                .FirstOrDefaultAsync(mt => mt.Id == dto.MaintenanceTicketId.Value);

            if (maintenanceTicket == null)
            {
                throw new ArgumentException($"Maintenance ticket #{dto.MaintenanceTicketId.Value} not found");
            }

            if (!string.Equals(maintenanceTicket.StatusCode, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ tạo hóa đơn cho phiếu bảo dưỡng đã hoàn thành");
            }

            if (await _repository.ExistsByMaintenanceTicketIdAsync(maintenanceTicket.Id))
            {
                throw new InvalidOperationException("Phiếu bảo dưỡng này đã có hóa đơn");
            }

            var entity = _mapper.Map<TotalReceipt>(dto);
            entity.MaintenanceTicketId = maintenanceTicket.Id;
            entity.CarId = dto.CarId ?? maintenanceTicket.CarId;
            entity.BranchId = dto.BranchId ?? maintenanceTicket.BranchId;
            entity.CreatedAt = dto.CreatedAt ?? DateTime.UtcNow;
            entity.CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? "VND" : dto.CurrencyCode;
            entity.StatusCode = string.IsNullOrWhiteSpace(dto.StatusCode) ? "PENDING" : dto.StatusCode;

            ApplyFinancials(entity, dto);

            var created = await _repository.AddAsync(entity);
            return MapToResponse(created);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<ResponseDto?> GetByMaintenanceTicketIdAsync(long maintenanceTicketId)
        {
            var entity = await _repository.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<PagedResultDto<ResponseDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? statusCode = null, DateTime? fromDate = null, DateTime? toDate = null, long? branchId = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var list = await _repository.GetListAsync(statusCode, branchId, fromDate, toDate);
            var mapped = list.Select(MapToResponse).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLowerInvariant();
                mapped = mapped.Where(r =>
                    (!string.IsNullOrWhiteSpace(r.Code) && r.Code.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrWhiteSpace(r.MaintenanceTicketCode) && r.MaintenanceTicketCode.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrWhiteSpace(r.CustomerName) && r.CustomerName.ToLowerInvariant().Contains(keyword)) ||
                    (!string.IsNullOrWhiteSpace(r.CustomerPhone) && r.CustomerPhone.Contains(keyword)) ||
                    (!string.IsNullOrWhiteSpace(r.LicensePlate) && r.LicensePlate.ToLowerInvariant().Contains(keyword)))
                    .ToList();
            }

            var total = mapped.Count;
            var items = mapped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDto<ResponseDto>
            {
                Items = items,
                TotalItems = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            if (dto.MaintenanceTicketId.HasValue && dto.MaintenanceTicketId.Value != existing.MaintenanceTicketId)
            {
                var maintenanceTicket = await _context.MaintenanceTickets
                    .Include(mt => mt.Car)
                        .ThenInclude(c => c.User)
                    .Include(mt => mt.Branch)
                    .FirstOrDefaultAsync(mt => mt.Id == dto.MaintenanceTicketId.Value);

                if (maintenanceTicket == null)
                {
                    throw new ArgumentException($"Maintenance ticket #{dto.MaintenanceTicketId.Value} not found");
                }

                if (!string.Equals(maintenanceTicket.StatusCode, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Chỉ gán hóa đơn cho phiếu bảo dưỡng đã hoàn thành");
                }

                if (await _repository.ExistsByMaintenanceTicketIdAsync(maintenanceTicket.Id, id))
                {
                    throw new InvalidOperationException("Phiếu bảo dưỡng này đã có hóa đơn");
                }

                existing.MaintenanceTicketId = maintenanceTicket.Id;
                existing.CarId = maintenanceTicket.CarId;
                existing.BranchId = maintenanceTicket.BranchId;
            }

            existing.CarId = dto.CarId ?? existing.CarId;
            existing.BranchId = dto.BranchId ?? existing.BranchId;
            existing.AccountantId = dto.AccountantId ?? existing.AccountantId;
            existing.CurrencyCode = string.IsNullOrWhiteSpace(dto.CurrencyCode) ? (existing.CurrencyCode ?? "VND") : dto.CurrencyCode;
            existing.StatusCode = string.IsNullOrWhiteSpace(dto.StatusCode) ? existing.StatusCode : dto.StatusCode;
            existing.Note = dto.Note ?? existing.Note;
            existing.CreatedAt = dto.CreatedAt ?? existing.CreatedAt ?? DateTime.UtcNow;

            ApplyFinancials(existing, dto);

            var updated = await _repository.UpdateAsync(existing);
            return MapToResponse(updated);
        }

        private void ApplyFinancials(TotalReceipt entity, RequestDto dto)
        {
            decimal subtotal = dto.Subtotal
                ?? entity.Subtotal
                ?? dto.Amount
                ?? entity.Amount;

            var vatPercent = dto.VatPercent ?? entity.VatPercent ?? 0m;
            var vatAmount = dto.VatAmount ?? entity.VatAmount ?? (vatPercent * subtotal / 100m);
            var surcharge = dto.SurchargeAmount ?? entity.SurchargeAmount ?? 0m;
            var discount = dto.DiscountAmount ?? entity.DiscountAmount ?? 0m;
            var final = dto.FinalAmount ?? entity.FinalAmount ?? (subtotal + vatAmount + surcharge - discount);

            entity.Subtotal = subtotal;
            entity.VatPercent = vatPercent;
            entity.VatAmount = vatAmount;
            entity.DiscountAmount = discount;
            entity.SurchargeAmount = surcharge;
            entity.FinalAmount = final;
            entity.Amount = dto.Amount ?? final;
        }

        private ResponseDto MapToResponse(TotalReceipt entity)
        {
            var dto = _mapper.Map<ResponseDto>(entity);
            dto.Code = EnsureInvoiceFormat(dto.Code, GenerateInvoiceCode(entity.Id));
            dto.MaintenanceTicketCode = entity.MaintenanceTicket?.Code;

            var car = entity.Car ?? entity.MaintenanceTicket?.Car;
            if (car != null)
            {
                dto.CarId = car.Id;
                dto.CarName = car.CarName;
                dto.LicensePlate = car.LicensePlate;
                if (car.User != null)
                {
                    var fullName = $"{car.User.FirstName} {car.User.LastName}".Trim();
                    dto.CustomerName = string.IsNullOrWhiteSpace(fullName) ? car.User.Username : fullName;
                    dto.CustomerPhone = car.User.Phone;
                    dto.CustomerEmail = car.User.Email;
                }
            }

            dto.BranchId = entity.BranchId ?? entity.MaintenanceTicket?.BranchId;
            dto.BranchName = entity.Branch?.Name ?? entity.MaintenanceTicket?.Branch?.Name;
            dto.AccountantName = entity.Accountant != null
                ? $"{entity.Accountant.FirstName} {entity.Accountant.LastName}".Trim()
                : null;
            dto.StatusName = entity.StatusCodeNavigation?.Name;
            dto.CurrencyCode ??= entity.CurrencyCode ?? "VND";
            dto.FinalAmount ??= entity.FinalAmount ?? entity.Amount;
            dto.Amount ??= entity.Amount;
            dto.Subtotal ??= entity.Subtotal;
            dto.VatAmount ??= entity.VatAmount;
            dto.VatPercent ??= entity.VatPercent;
            dto.SurchargeAmount ??= entity.SurchargeAmount;
            dto.DiscountAmount ??= entity.DiscountAmount;
            dto.CreatedAt ??= entity.CreatedAt;

            return dto;
        }

        private static string EnsureInvoiceFormat(string? existing, string fallback)
        {
            if (string.IsNullOrWhiteSpace(existing)) return fallback;
            var normalized = existing.Trim().ToUpperInvariant();
            return normalized.StartsWith("INV") && normalized.Length == 9 ? normalized : fallback;
        }

        private static string GenerateInvoiceCode(long seed)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var value = seed > 0 ? seed : DateTime.UtcNow.Ticks;
            var result = string.Empty;
            for (var i = 0; i < 6; i++)
            {
                value = (value * 9301 + 49297) % 233280;
                result += chars[(int)(value % chars.Length)];
            }
            return $"INV{result}";
        }
    }
}


