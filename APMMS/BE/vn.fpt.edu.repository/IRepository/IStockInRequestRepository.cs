using System.Collections.Generic;
using System.Threading.Tasks;
using BE.vn.fpt.edu.models;

namespace BE.vn.fpt.edu.repository.IRepository
{
    public interface IStockInRequestRepository
    {
        Task<IEnumerable<StockInRequest>> GetAllAsync(int page = 1, int pageSize = 10, long? branchId = null, string? statusCode = null, string? search = null);
        Task<int> GetTotalCountAsync(long? branchId = null, string? statusCode = null, string? search = null);
        Task<StockInRequest?> GetByIdAsync(long id);
        Task<StockInRequest?> GetByCodeAsync(string code);
        Task<IEnumerable<StockInRequest>> GetByStatusAsync(string statusCode);
        Task<StockInRequest> AddAsync(StockInRequest entity);
        Task<StockInRequest> UpdateAsync(StockInRequest entity);
        Task<bool> ExistsAsync(long id);
        Task<bool> CodeExistsAsync(string code, long? excludeId = null);
    }
}

