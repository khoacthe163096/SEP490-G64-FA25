using FE.vn.fpt.edu.adapters;
using System.Web;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.services
{
    public class ComponentService
    {
        private readonly ApiAdapter _api;
        private const string Base = "Component";
        private const string TypeBase = "TypeComponent";

        public ComponentService(ApiAdapter api)
        {
            _api = api;
        }

        public async Task<IEnumerable<ComponentViewModel>> GetAllAsync(string? search = null, long? branchId = null, long? typeComponentId = null, string? statusCode = null)
        {
            var q = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) q.Add($"search={HttpUtility.UrlEncode(search)}");
            if (branchId.HasValue) q.Add($"branchId={branchId.Value}");
            if (typeComponentId.HasValue) q.Add($"typeComponentId={typeComponentId.Value}");
            if (!string.IsNullOrWhiteSpace(statusCode)) q.Add($"statusCode={HttpUtility.UrlEncode(statusCode)}");

            var endpoint = Base + (q.Count > 0 ? "?" + string.Join("&", q) : "");
            return await _api.GetAsync<List<ComponentViewModel>>(endpoint) ?? new List<ComponentViewModel>();
        }

        public async Task<ComponentViewModel?> GetByIdAsync(long id)
        {
            return await _api.GetAsync<ComponentViewModel>($"{Base}/{id}");
        }

        public async Task<ComponentViewModel?> CreateAsync(ComponentViewModel payload)
        {
            return await _api.PostAsync<ComponentViewModel>(Base, payload);
        }

        public async Task<bool> UpdateAsync(long id, ComponentViewModel payload)
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

        public async Task<IEnumerable<TypeComponentLookupViewModel>> GetTypeComponentsAsync()
        {
            return await _api.GetAsync<List<TypeComponentLookupViewModel>>(TypeBase) ?? new List<TypeComponentLookupViewModel>();
        }
    }
}
