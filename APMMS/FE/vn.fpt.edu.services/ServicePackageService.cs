using FE.vn.fpt.edu.adapters;
using System.Web;
using vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.services
{
    public class ServicePackageService
    {
        private readonly ApiAdapter _api;
        private const string Base = "ServicePackage";

        public ServicePackageService(ApiAdapter api)
        {
            _api = api;
        }

        public async Task<IEnumerable<ServicePackageViewModel>> GetAllAsync(string? search = null, string? statusCode = null)
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={HttpUtility.UrlEncode(search)}");
            if (!string.IsNullOrWhiteSpace(statusCode)) query.Add($"statusCode={HttpUtility.UrlEncode(statusCode)}");
            var endpoint = Base + (query.Count > 0 ? "?" + string.Join("&", query) : "");
            return await _api.GetAsync<List<ServicePackageViewModel>>(endpoint) ?? new List<ServicePackageViewModel>();
        }

        public async Task<ServicePackageViewModel?> GetByIdAsync(long id)
        {
            return await _api.GetAsync<ServicePackageViewModel>($"{Base}/{id}");
        }

        public async Task<ServicePackageViewModel?> CreateAsync(ServicePackageViewModel payload)
        {
            return await _api.PostAsync<ServicePackageViewModel>(Base, payload);
        }

        public async Task<bool> UpdateAsync(long id, ServicePackageViewModel payload)
        {
            return await _api.PutAsync($"{Base}/{id}", payload);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _api.DeleteAsync($"{Base}/{id}");
        }
    }
}
