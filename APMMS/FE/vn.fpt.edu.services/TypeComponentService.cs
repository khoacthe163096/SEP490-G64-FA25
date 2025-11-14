using FE.vn.fpt.edu.adapters;
using System.Web;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.services
{
    public class TypeComponentService
    {
        private readonly ApiAdapter _api;
        private const string Base = "TypeComponent";

        public TypeComponentService(ApiAdapter api)
        {
            _api = api;
        }

        public async Task<IEnumerable<TypeComponentLookupViewModel>> GetAllLookupAsync()
        {
            return await _api.GetAsync<List<TypeComponentLookupViewModel>>(Base) ?? new List<TypeComponentLookupViewModel>();
        }

        public async Task<IEnumerable<TypeComponentViewModel>> GetAllAsync(string? search = null, long? branchId = null, string? statusCode = null)
        {
            var q = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) q.Add($"search={HttpUtility.UrlEncode(search)}");
            if (branchId.HasValue) q.Add($"branchId={branchId.Value}");
            if (!string.IsNullOrWhiteSpace(statusCode)) q.Add($"statusCode={HttpUtility.UrlEncode(statusCode)}");

            var endpoint = Base + (q.Count > 0 ? "?" + string.Join("&", q) : "");
            return await _api.GetAsync<List<TypeComponentViewModel>>(endpoint) ?? new List<TypeComponentViewModel>();
        }

        public async Task<TypeComponentViewModel?> GetByIdAsync(long id)
        {
            return await _api.GetAsync<TypeComponentViewModel>($"{Base}/{id}");
        }

        public async Task<TypeComponentViewModel?> CreateAsync(TypeComponentViewModel payload)
        {
            return await _api.PostAsync<TypeComponentViewModel>(Base, payload);
        }

        public async Task<bool> UpdateAsync(long id, TypeComponentViewModel payload)
        {
            return await _api.PutAsync($"{Base}/{id}", payload);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _api.DeleteAsync($"{Base}/{id}");
        }

        public async Task<bool> ToggleStatusAsync(long id, string statusCode)
        {
            return await _api.PutAsync($"{Base}/{id}/status?statusCode={HttpUtility.UrlEncode(statusCode)}", new { });
        }
    }
}
