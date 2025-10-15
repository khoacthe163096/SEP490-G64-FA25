using FE.vn.fpt.edu.adapters;
using FE.vn.fpt.edu.viewmodels;

namespace FE.vn.fpt.edu.services
{
    public class AuthService
    {
        private readonly ApiAdapter _apiAdapter;

        public AuthService(ApiAdapter apiAdapter)
        {
            _apiAdapter = apiAdapter;
        }

        public async Task<LoginResponseModel?> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"AuthService: Attempting login for user: {username}");
                
                var loginRequest = new LoginRequestModel
                {
                    Username = username,
                    Password = password
                };

                var result = await _apiAdapter.PostAsync<LoginResponseModel>("Auth/login", loginRequest);
                Console.WriteLine($"AuthService: API call result - Success: {result?.Success}, RoleId: {result?.RoleId}, RedirectTo: {result?.RedirectTo}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Exception during login: {ex.Message}");
                return new LoginResponseModel { Success = false, Error = ex.Message };
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                // Add JWT token to request header
                var token = GetStoredToken();
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                // For now, just clear local storage
                // In a real implementation, you would call the logout API
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RegisterResponseModel?> RegisterAsync(RegisterRequestModel request)
        {
            return await _apiAdapter.PostAsync<RegisterResponseModel>("auth/register", request);
        }

        public string? GetStoredToken()
        {
            // This would typically get the token from a secure storage
            // For now, we'll use a simple approach
            return null;
        }

        public void StoreToken(string token)
        {
            // Store token in secure storage
            // For now, we'll use localStorage via JavaScript
        }

        public void ClearToken()
        {
            // Clear token from storage
        }
    }
}
