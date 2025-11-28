using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.vn.fpt.edu.DTOs.TicketComponent;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class TicketComponentService : ITicketComponentService
    {
        private readonly ITicketComponentRepository _repository;
        private readonly CarMaintenanceDbContext _context;
        private readonly IHistoryLogRepository _historyLogRepository;

        public TicketComponentService(ITicketComponentRepository repository, CarMaintenanceDbContext context, IHistoryLogRepository historyLogRepository)
        {
            _repository = repository;
            _context = context;
            _historyLogRepository = historyLogRepository;
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto, long? userId = null)
        {
            // Validate MaintenanceTicket exists and get BranchId
            var ticket = await _context.MaintenanceTickets
                .Include(mt => mt.Branch)
                .FirstOrDefaultAsync(mt => mt.Id == dto.MaintenanceTicketId);
            if (ticket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Không cho phép thêm component vào phiếu đã hoàn thành hoặc đã hủy
            if (ticket.StatusCode == "COMPLETED" || ticket.StatusCode == "CANCELLED")
                throw new ArgumentException("Không thể thêm component vào phiếu đã hoàn thành hoặc đã hủy");

            // Validate Component exists
            var component = await _context.Components
                .Include(c => c.TypeComponent)
                .FirstOrDefaultAsync(c => c.Id == dto.ComponentId);
            if (component == null)
                throw new ArgumentException("Component not found");

            // ✅ VALIDATION: Component phải thuộc cùng Branch với MaintenanceTicket
            if (component.BranchId != ticket.BranchId)
                throw new ArgumentException("Component không thuộc chi nhánh của phiếu bảo dưỡng");

            // ✅ VALIDATION: Component phải ở trạng thái ACTIVE
            if (component.StatusCode != "ACTIVE")
                throw new ArgumentException("Component không khả dụng (trạng thái không phải ACTIVE)");

            // ✅ VALIDATION: Kiểm tra số lượng tồn kho
            if (component.QuantityStock.HasValue && dto.Quantity > component.QuantityStock.Value)
                throw new ArgumentException($"Không đủ số lượng tồn kho. Hiện có: {component.QuantityStock.Value}, yêu cầu: {dto.Quantity}");

            // Check if already exists
            var existing = await _context.TicketComponents
                .FirstOrDefaultAsync(tc => tc.MaintenanceTicketId == dto.MaintenanceTicketId 
                    && tc.ComponentId == dto.ComponentId);
            if (existing != null)
                throw new ArgumentException("Component already added to this ticket");

            // ✅ QUY TRÌNH: Giá trong phiếu PHẢI theo giá xuất (UnitPrice) trong kho - KHÔNG cho phép chỉnh
            // Giảm giá sẽ được áp dụng ở hóa đơn (TotalReceipt.DiscountAmount), không chỉnh giá từng component
            
            // Luôn lấy giá xuất từ Component, bỏ qua UnitPrice từ DTO (nếu có)
            var unitPrice = component.UnitPrice ?? 0;
            
            // ✅ VALIDATION: Không cho phép chỉnh giá trong phiếu
            if (dto.UnitPrice.HasValue && component.UnitPrice.HasValue && dto.UnitPrice.Value != component.UnitPrice.Value)
            {
                throw new ArgumentException($"Không được phép chỉnh đơn giá trong phiếu bảo dưỡng. Giá phải theo giá xuất trong kho: {component.UnitPrice.Value:N0} ₫. Nếu cần giảm giá, vui lòng sử dụng phần giảm giá trong hóa đơn.");
            }

            // ✅ Trừ số lượng tồn kho
            if (component.QuantityStock.HasValue)
            {
                component.QuantityStock -= dto.Quantity;
                if (component.QuantityStock < 0)
                    component.QuantityStock = 0;
                _context.Components.Update(component);
            }

            var entity = new TicketComponent
            {
                MaintenanceTicketId = dto.MaintenanceTicketId,
                ComponentId = dto.ComponentId,
                BranchId = ticket.BranchId, // ✅ Tự động set BranchId từ MaintenanceTicket
                Quantity = dto.Quantity,
                ActualQuantity = dto.ActualQuantity ?? (decimal?)dto.Quantity, // Nếu không có ActualQuantity thì lấy bằng Quantity
                UnitPrice = unitPrice,
                ServicePackageId = dto.ServicePackageId // ✅ Đánh dấu phụ tùng từ gói dịch vụ
            };

            var created = await _repository.AddAsync(entity);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(dto.MaintenanceTicketId);
            
            // ✅ Tạo history log để ghi nhận việc thêm phụ tùng
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var userName = user != null 
                    ? ($"{user.FirstName} {user.LastName}").Trim() 
                    : "Unknown";
                
                var componentDetails = $"Thêm phụ tùng '{component.Name}' (Mã: {component.Code})";
                componentDetails += $" - Số lượng: {dto.Quantity}";
                if (dto.UnitPrice.HasValue)
                {
                    componentDetails += $" - Đơn giá: {dto.UnitPrice.Value:N0} ₫";
                    componentDetails += $" - Thành tiền: {(dto.Quantity * dto.UnitPrice.Value):N0} ₫";
                }
                
                await CreateHistoryLogAsync(
                    userId: userId,
                    action: "ADD_COMPONENT",
                    maintenanceTicketId: dto.MaintenanceTicketId,
                    newData: $"{componentDetails} bởi {userName}"
                );
            }
            
            return MapToResponse(created);
        }

        public async Task<ResponseDto?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToResponse(entity);
        }

        public async Task<IEnumerable<ResponseDto>> GetByMaintenanceTicketIdAsync(long maintenanceTicketId)
        {
            var entities = await _repository.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
            return entities.Select(MapToResponse);
        }

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto dto, long? userId = null)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Validate MaintenanceTicket exists and get status
            var ticket = await _context.MaintenanceTickets
                .FirstOrDefaultAsync(mt => mt.Id == existing.MaintenanceTicketId);
            if (ticket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // ✅ VALIDATION: Không cho phép cập nhật component trong phiếu đã hoàn thành hoặc đã hủy
            if (ticket.StatusCode == "COMPLETED" || ticket.StatusCode == "CANCELLED")
                throw new ArgumentException("Không thể cập nhật component trong phiếu đã hoàn thành hoặc đã hủy");

            // Validate Component exists if changed
            Component? component = null;
            if (existing.ComponentId != dto.ComponentId)
            {
                component = await _context.Components
                    .Include(c => c.TypeComponent)
                    .FirstOrDefaultAsync(c => c.Id == dto.ComponentId);
                if (component == null)
                    throw new ArgumentException("Component not found");

                // ✅ VALIDATION: Component mới phải thuộc cùng Branch với MaintenanceTicket
                if (component.BranchId != ticket.BranchId)
                    throw new ArgumentException("Component không thuộc chi nhánh của phiếu bảo dưỡng");

                // ✅ VALIDATION: Component phải ở trạng thái ACTIVE
                if (component.StatusCode != "ACTIVE")
                    throw new ArgumentException("Component không khả dụng (trạng thái không phải ACTIVE)");
            }

            // Get component for price validation
            if (component == null)
            {
                component = await _context.Components.FindAsync(dto.ComponentId);
            }

            // ✅ QUY TRÌNH: Giá trong phiếu PHẢI theo giá xuất (UnitPrice) trong kho - KHÔNG cho phép chỉnh
            // Giảm giá sẽ được áp dụng ở hóa đơn (TotalReceipt.DiscountAmount), không chỉnh giá từng component
            
            // Luôn lấy giá xuất từ Component
            var defaultUnitPrice = component?.UnitPrice ?? 0;
            
            // ✅ VALIDATION: Không cho phép chỉnh giá trong phiếu
            if (dto.UnitPrice.HasValue && component != null && component.UnitPrice.HasValue && dto.UnitPrice.Value != component.UnitPrice.Value)
            {
                throw new ArgumentException($"Không được phép chỉnh đơn giá trong phiếu bảo dưỡng. Giá phải theo giá xuất trong kho: {component.UnitPrice.Value:N0} ₫. Nếu cần giảm giá, vui lòng sử dụng phần giảm giá trong hóa đơn.");
            }
            
            // Luôn dùng giá xuất từ Component
            dto.UnitPrice = defaultUnitPrice;

            // ✅ VALIDATION: Kiểm tra số lượng tồn kho (nếu thay đổi component hoặc quantity)
            if (component != null && (existing.ComponentId != dto.ComponentId || existing.Quantity != dto.Quantity))
            {
                // Tính số lượng cần thêm/bớt
                var quantityDiff = dto.Quantity - existing.Quantity;
                
                if (existing.ComponentId != dto.ComponentId)
                {
                    // Đổi component: hoàn trả component cũ, trừ component mới
                    var oldComponent = await _context.Components.FindAsync(existing.ComponentId);
                    if (oldComponent != null && oldComponent.QuantityStock.HasValue)
                    {
                        oldComponent.QuantityStock += existing.Quantity;
                        _context.Components.Update(oldComponent);
                    }

                    // Kiểm tra component mới có đủ tồn kho
                    if (component.QuantityStock.HasValue && dto.Quantity > component.QuantityStock.Value)
                        throw new ArgumentException($"Không đủ số lượng tồn kho. Hiện có: {component.QuantityStock.Value}, yêu cầu: {dto.Quantity}");

                    // Trừ component mới
                    if (component.QuantityStock.HasValue)
                    {
                        component.QuantityStock -= dto.Quantity;
                        if (component.QuantityStock < 0)
                            component.QuantityStock = 0;
                        _context.Components.Update(component);
                    }
                }
                else if (quantityDiff != 0)
                {
                    // Chỉ thay đổi số lượng: điều chỉnh tồn kho
                    if (component.QuantityStock.HasValue)
                    {
                        var newStock = component.QuantityStock.Value - quantityDiff;
                        if (newStock < 0)
                            throw new ArgumentException($"Không đủ số lượng tồn kho. Hiện có: {component.QuantityStock.Value}, yêu cầu thêm: {quantityDiff}");

                        component.QuantityStock = newStock;
                        _context.Components.Update(component);
                    }
                }
            }

            existing.ComponentId = dto.ComponentId;
            
            // ✅ XỬ LÝ: Nếu số lượng thực tế > số lượng đã lấy, tự động cập nhật và trừ thêm từ kho
            if (dto.ActualQuantity.HasValue && dto.ActualQuantity.Value > dto.Quantity)
            {
                var additionalNeeded = dto.ActualQuantity.Value - dto.Quantity;
                var componentForUpdate = await _context.Components.FindAsync(existing.ComponentId);
                
                if (componentForUpdate == null)
                    throw new ArgumentException("Component not found");
                
                // Kiểm tra tồn kho còn đủ không (convert decimal sang int)
                var additionalNeededInt = (int)Math.Ceiling(additionalNeeded);
                if (!componentForUpdate.QuantityStock.HasValue || componentForUpdate.QuantityStock.Value < additionalNeededInt)
                {
                    throw new ArgumentException(
                        $"Không đủ số lượng trong kho. Cần thêm: {additionalNeededInt}, Tồn kho: {componentForUpdate.QuantityStock ?? 0}");
                }
                
                // Tự động cập nhật Quantity = ActualQuantity (convert decimal sang int) và trừ thêm từ kho
                existing.Quantity = (int)Math.Ceiling(dto.ActualQuantity.Value);
                componentForUpdate.QuantityStock = (componentForUpdate.QuantityStock ?? 0) - additionalNeededInt;
                if (componentForUpdate.QuantityStock < 0)
                    componentForUpdate.QuantityStock = 0;
                _context.Components.Update(componentForUpdate);
                
                // Ghi log: "Đã lấy thêm X phụ tùng vì dùng nhiều hơn dự kiến"
                if (userId.HasValue)
                {
                    var user = await _context.Users.FindAsync(userId.Value);
                    var userName = user != null 
                        ? ($"{user.FirstName} {user.LastName}").Trim() 
                        : "Unknown";
                    
                    await CreateHistoryLogAsync(
                        userId: userId,
                        action: "UPDATE_COMPONENT_QUANTITY",
                        maintenanceTicketId: existing.MaintenanceTicketId,
                        newData: $"Đã lấy thêm {additionalNeededInt} phụ tùng '{componentForUpdate.Name}' (tổng: {existing.Quantity}) bởi {userName}"
                    );
                }
            }
            else
            {
                existing.Quantity = dto.Quantity;
            }
            
            // Cập nhật ActualQuantity: nếu có giá trị thì dùng, nếu null thì set bằng Quantity
            existing.ActualQuantity = dto.ActualQuantity ?? (decimal?)existing.Quantity;
            existing.UnitPrice = dto.UnitPrice;
            // ✅ BranchId không thay đổi khi update (luôn theo MaintenanceTicket)
            // Đảm bảo BranchId luôn được set từ MaintenanceTicket (trường hợp data cũ có thể chưa có)
            if (existing.BranchId != ticket.BranchId)
            {
                existing.BranchId = ticket.BranchId;
            }

            // ✅ Ghi log chi tiết khi cập nhật phụ tùng
            var oldQuantity = existing.Quantity;
            var oldActualQuantity = existing.ActualQuantity;
            var oldUnitPrice = existing.UnitPrice;
            
            var updated = await _repository.UpdateAsync(existing);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(existing.MaintenanceTicketId ?? 0);
            
            // ✅ Tạo history log chi tiết khi cập nhật
            if (userId.HasValue && updated != null)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var userName = user != null 
                    ? ($"{user.FirstName} {user.LastName}").Trim() 
                    : "Unknown";
                
                var componentForLog = await _context.Components.FindAsync(updated.ComponentId);
                var componentName = componentForLog?.Name ?? "N/A";
                
                var changes = new List<string>();
                
                if (oldQuantity != updated.Quantity)
                {
                    changes.Add($"Số lượng: {oldQuantity} → {updated.Quantity}");
                }
                
                if (oldActualQuantity != updated.ActualQuantity)
                {
                    changes.Add($"Số lượng thực tế: {oldActualQuantity} → {updated.ActualQuantity}");
                }
                
                if (oldUnitPrice != updated.UnitPrice)
                {
                    changes.Add($"Đơn giá: {oldUnitPrice:N0} ₫ → {updated.UnitPrice:N0} ₫");
                }
                
                var changeDetails = changes.Any() 
                    ? string.Join("; ", changes)
                    : "Không có thay đổi";
                
                await CreateHistoryLogAsync(
                    userId: userId,
                    action: "UPDATE_COMPONENT",
                    maintenanceTicketId: updated.MaintenanceTicketId,
                    oldData: $"Phụ tùng '{componentName}': Số lượng={oldQuantity}, Số lượng thực tế={oldActualQuantity}, Đơn giá={oldUnitPrice:N0} ₫",
                    newData: $"Cập nhật phụ tùng '{componentName}': {changeDetails} bởi {userName}"
                );
            }
            
            return updated != null ? MapToResponse(updated) : null;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            // Validate MaintenanceTicket exists and get status
            var ticket = await _context.MaintenanceTickets
                .FirstOrDefaultAsync(mt => mt.Id == entity.MaintenanceTicketId);
            if (ticket != null)
            {
                // ✅ VALIDATION: Không cho phép xóa component trong phiếu đã hoàn thành hoặc đã hủy
                if (ticket.StatusCode == "COMPLETED" || ticket.StatusCode == "CANCELLED")
                    throw new ArgumentException("Không thể xóa component trong phiếu đã hoàn thành hoặc đã hủy");
            }
            
            // ✅ VALIDATION: Không cho phép xóa phụ tùng từ gói dịch vụ
            if (entity.ServicePackageId.HasValue)
                throw new ArgumentException("Không thể xóa phụ tùng từ gói dịch vụ. Nếu muốn xóa, vui lòng xóa toàn bộ gói dịch vụ khỏi phiếu bảo dưỡng.");

            // ✅ Ghi log chi tiết trước khi xóa
            var componentForDelete = await _context.Components.FindAsync(entity.ComponentId);
            var componentName = componentForDelete?.Name ?? "N/A";
            var componentCode = componentForDelete?.Code ?? "N/A";
            
            // ✅ Hoàn trả component về tồn kho
            if (componentForDelete != null && componentForDelete.QuantityStock.HasValue)
            {
                // Hoàn trả số lượng đã lấy (dùng Quantity, không phải ActualQuantity vì có thể đã dùng rồi)
                // Hoặc có thể hoàn trả ActualQuantity nếu chưa dùng hết
                // Ở đây hoàn trả Quantity để đơn giản
                componentForDelete.QuantityStock += entity.Quantity;
                _context.Components.Update(componentForDelete);
            }
            
            var maintenanceTicketId = entity.MaintenanceTicketId ?? 0;
            
            // ✅ Ghi log chi tiết trước khi xóa
            var deleteDetails = $"Xóa phụ tùng '{componentName}' (Mã: {componentCode}) - Số lượng: {entity.Quantity}, Số lượng thực tế: {entity.ActualQuantity}, Đơn giá: {entity.UnitPrice:N0} ₫";
            
            var result = await _repository.DeleteAsync(id);
            
            if (result)
            {
                // Cập nhật TotalEstimatedCost của MaintenanceTicket
                await UpdateMaintenanceTicketTotalCost(maintenanceTicketId);
                
                // ✅ Tạo history log chi tiết khi xóa
                await CreateHistoryLogAsync(
                    userId: null, // Có thể lấy từ ticket nếu cần
                    action: "DELETE_COMPONENT",
                    maintenanceTicketId: maintenanceTicketId,
                    oldData: deleteDetails,
                    newData: "Đã xóa phụ tùng khỏi phiếu bảo dưỡng"
                );
            }
            
            return result;
        }

        public async Task<decimal> CalculateTotalCostAsync(long maintenanceTicketId)
        {
            var ticketComponents = await _repository.GetByMaintenanceTicketIdAsync(maintenanceTicketId);
            // Tính chi phí dựa trên ActualQuantity (nếu có) hoặc Quantity
            return ticketComponents.Sum(tc => 
            {
                var quantity = tc.ActualQuantity ?? tc.Quantity;
                return quantity * (tc.UnitPrice ?? 0);
            });
        }

        /// <summary>
        /// Cập nhật TotalEstimatedCost của MaintenanceTicket = ComponentTotal + LaborCostTotal
        /// </summary>
        private async Task UpdateMaintenanceTicketTotalCost(long maintenanceTicketId)
        {
            var maintenanceTicket = await _context.MaintenanceTickets
                .Include(mt => mt.TicketComponents)
                .Include(mt => mt.ServiceTasks)
                .FirstOrDefaultAsync(mt => mt.Id == maintenanceTicketId);
            
            if (maintenanceTicket == null)
                return;
            
            // Tính tổng phụ tùng dựa trên ActualQuantity (nếu có) hoặc Quantity
            var componentTotal = maintenanceTicket.TicketComponents
                .Sum(tc => 
                {
                    var quantity = tc.ActualQuantity ?? tc.Quantity;
                    return quantity * (tc.UnitPrice ?? 0);
                });
            
            // Tính tổng phí nhân công
            var laborCostTotal = maintenanceTicket.ServiceTasks
                .Sum(st => st.LaborCost ?? 0);
            
            // Cập nhật TotalEstimatedCost
            maintenanceTicket.TotalEstimatedCost = componentTotal + laborCostTotal;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Helper method để tạo history log
        /// </summary>
        private async Task CreateHistoryLogAsync(long? userId, string action, long? maintenanceTicketId = null, string? oldData = null, string? newData = null)
        {
            var historyLog = new HistoryLog
            {
                UserId = userId,
                MaintenanceTicketId = maintenanceTicketId,
                Action = action,
                OldData = oldData,
                NewData = newData,
                CreatedAt = DateTime.UtcNow
            };

            await _historyLogRepository.CreateAsync(historyLog);
        }

        private ResponseDto MapToResponse(TicketComponent entity)
        {
            // Tính TotalPrice dựa trên ActualQuantity (nếu có) hoặc Quantity
            var quantity = entity.ActualQuantity ?? entity.Quantity;
            var totalPrice = quantity * (entity.UnitPrice ?? 0);
            
            return new ResponseDto
            {
                Id = entity.Id,
                MaintenanceTicketId = entity.MaintenanceTicketId ?? 0,
                ComponentId = entity.ComponentId ?? 0,
                ServicePackageId = entity.ServicePackageId, // ✅ Map ServicePackageId
                Quantity = entity.Quantity,
                ActualQuantity = entity.ActualQuantity,
                UnitPrice = entity.UnitPrice,
                TotalPrice = totalPrice,
                ComponentName = entity.Component?.Name,
                ComponentCode = entity.Component?.Code,
                ComponentImageUrl = entity.Component?.ImageUrl,
                TypeComponentName = entity.Component?.TypeComponent?.Name
            };
        }
    }
}

