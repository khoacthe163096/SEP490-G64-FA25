using FE.vn.fpt.edu.adapters;

namespace FE.vn.fpt.edu.services
{
    public class MaintenanceTicketService
    {
        private readonly ApiAdapter _apiAdapter;

        public MaintenanceTicketService(ApiAdapter apiAdapter)
        {
            _apiAdapter = apiAdapter;
        }

        public async Task<object?> GetAllAsync(int page, int pageSize)
        {
            return await _apiAdapter.GetAsync<object>($"MaintenanceTicket?page={page}&pageSize={pageSize}");
        }

        public async Task<object?> CreateFromCheckinAsync(object payload)
        {
            return await _apiAdapter.PostAsync<object>("MaintenanceTicket/create-from-checkin", payload);
        }
    }
}


