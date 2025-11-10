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
            // Validate MaintenanceTicket exists
            var ticket = await _context.MaintenanceTickets.FindAsync(dto.MaintenanceTicketId);
            if (ticket == null)
                throw new ArgumentException("Maintenance ticket not found");

            // Validate Component exists
            var component = await _context.Components
                .Include(c => c.TypeComponent)
                .FirstOrDefaultAsync(c => c.Id == dto.ComponentId);
            if (component == null)
                throw new ArgumentException("Component not found");

            // Check if already exists
            var existing = await _context.TicketComponents
                .FirstOrDefaultAsync(tc => tc.MaintenanceTicketId == dto.MaintenanceTicketId 
                    && tc.ComponentId == dto.ComponentId);
            if (existing != null)
                throw new ArgumentException("Component already added to this ticket");

            // Use component's unit price if not provided
            var unitPrice = dto.UnitPrice ?? component.UnitPrice ?? 0;

            var entity = new TicketComponent
            {
                MaintenanceTicketId = dto.MaintenanceTicketId,
                ComponentId = dto.ComponentId,
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

            // Validate Component exists if changed
            if (existing.ComponentId != dto.ComponentId)
            {
                var component = await _context.Components
                    .Include(c => c.TypeComponent)
                    .FirstOrDefaultAsync(c => c.Id == dto.ComponentId);
                if (component == null)
                    throw new ArgumentException("Component not found");
            }

            // Use component's unit price if not provided
            if (!dto.UnitPrice.HasValue)
            {
                var component = await _context.Components.FindAsync(dto.ComponentId);
                dto.UnitPrice = component?.UnitPrice ?? 0;
            }

            existing.ComponentId = dto.ComponentId;
            existing.Quantity = dto.Quantity;
            // Cập nhật ActualQuantity: nếu có giá trị thì dùng, nếu null thì set bằng Quantity
            existing.ActualQuantity = dto.ActualQuantity ?? (decimal?)dto.Quantity;
            existing.UnitPrice = dto.UnitPrice;

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

