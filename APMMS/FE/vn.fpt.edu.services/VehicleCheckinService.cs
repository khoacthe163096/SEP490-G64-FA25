using FE.vn.fpt.edu.adapters;

namespace FE.vn.fpt.edu.services
{
    public class VehicleCheckinService
    {
        private readonly ApiAdapter _apiAdapter;

        public VehicleCheckinService(ApiAdapter apiAdapter)
        {
            _apiAdapter = apiAdapter;
        }

        public async Task<object?> GetAllAsync(int page, int pageSize)
        {
            // Backend currently supports page and pageSize
            return await _apiAdapter.GetAsync<object>($"VehicleCheckin?page={page}&pageSize={pageSize}");
        }

        public async Task<object?> GetByIdAsync(long id)
        {
            try
            {
                var result = await _apiAdapter.GetAsync<object>($"VehicleCheckin/{id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return new { success = false, message = "Không thể tải dữ liệu chi tiết", error = ex.Message };
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _apiAdapter.DeleteAsync($"VehicleCheckin/{id}");
        }

        public async Task<object?> SearchVehicleAsync(string searchTerm)
        {
            return await _apiAdapter.GetAsync<object>($"VehicleCheckin/search-vehicle?searchTerm={Uri.EscapeDataString(searchTerm)}");
        }
    }
}


