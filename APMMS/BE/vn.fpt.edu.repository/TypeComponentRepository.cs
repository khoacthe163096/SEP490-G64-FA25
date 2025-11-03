using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.repository.IRepository;
namespace BE.vn.fpt.edu.repository
{
    public class TypeComponentRepository : ITypeComponentRepository
    {
        private readonly CarMaintenanceDbContext _context;
        public TypeComponentRepository(CarMaintenanceDbContext context)
        {
            _context = context;
        }

        public async Task<TypeComponent> AddAsync(TypeComponent entity)
        {
            var added = await _context.TypeComponents.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task DisableEnableAsync(long id, string statusCode)
        {
            var item = await _context.TypeComponents.FindAsync(id);
            if (item == null) return;
            item.StatusCode = statusCode;
            _context.TypeComponents.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.TypeComponents.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TypeComponent>> GetAllAsync(long? branchId = null, string? statusCode = null)
        {
            var q = _context.TypeComponents.AsQueryable();
            if (branchId.HasValue) q = q.Where(x => x.BranchId == branchId.Value);
            if (!string.IsNullOrEmpty(statusCode)) q = q.Where(x => x.StatusCode == statusCode);
            return await q.ToListAsync();
        }

        public async Task<TypeComponent?> GetByIdAsync(long id)
        {
            return await _context.TypeComponents.FindAsync(id);
        }

        public async Task<TypeComponent> UpdateAsync(TypeComponent entity)
        {
            _context.TypeComponents.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}