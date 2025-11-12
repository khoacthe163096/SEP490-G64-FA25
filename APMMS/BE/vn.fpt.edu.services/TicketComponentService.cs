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

        public TicketComponentService(ITicketComponentRepository repository, CarMaintenanceDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ResponseDto> CreateAsync(RequestDto dto)
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

            // Use component's unit price if not provided
            var unitPrice = dto.UnitPrice ?? component.UnitPrice ?? 0;

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
                UnitPrice = unitPrice
            };

            var created = await _repository.AddAsync(entity);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(dto.MaintenanceTicketId);
            
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

        public async Task<ResponseDto?> UpdateAsync(long id, RequestDto dto)
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

            // Use component's unit price if not provided
            if (!dto.UnitPrice.HasValue)
            {
                dto.UnitPrice = component?.UnitPrice ?? 0;
            }

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
            existing.Quantity = dto.Quantity;
            // Cập nhật ActualQuantity: nếu có giá trị thì dùng, nếu null thì set bằng Quantity
            existing.ActualQuantity = dto.ActualQuantity ?? (decimal?)dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;
            // ✅ BranchId không thay đổi khi update (luôn theo MaintenanceTicket)
            // Đảm bảo BranchId luôn được set từ MaintenanceTicket (trường hợp data cũ có thể chưa có)
            if (existing.BranchId != ticket.BranchId)
            {
                existing.BranchId = ticket.BranchId;
            }

            var updated = await _repository.UpdateAsync(existing);
            
            // Cập nhật TotalEstimatedCost của MaintenanceTicket
            await UpdateMaintenanceTicketTotalCost(existing.MaintenanceTicketId ?? 0);
            
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

            // ✅ Hoàn trả component về tồn kho
            var component = await _context.Components.FindAsync(entity.ComponentId);
            if (component != null && component.QuantityStock.HasValue)
            {
                // Hoàn trả số lượng đã lấy (dùng Quantity, không phải ActualQuantity vì có thể đã dùng rồi)
                // Hoặc có thể hoàn trả ActualQuantity nếu chưa dùng hết
                // Ở đây hoàn trả Quantity để đơn giản
                component.QuantityStock += entity.Quantity;
                _context.Components.Update(component);
            }
            
            var maintenanceTicketId = entity.MaintenanceTicketId ?? 0;
            var result = await _repository.DeleteAsync(id);
            
            if (result)
            {
                // Cập nhật TotalEstimatedCost của MaintenanceTicket
                await UpdateMaintenanceTicketTotalCost(maintenanceTicketId);
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

