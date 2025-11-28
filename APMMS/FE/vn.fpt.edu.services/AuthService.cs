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

                // Call backend API and get wrapped response
                var backendResponse = await _apiAdapter.PostAsync<BackendApiResponse<BackendLoginData>>("Auth/login", loginRequest);
                Console.WriteLine($"AuthService: Backend response - Success: {backendResponse?.Success}");
                Console.WriteLine($"AuthService: Backend response - Message: {backendResponse?.Message}");
                Console.WriteLine($"AuthService: Backend response - Data: {backendResponse?.Data != null}");
                
                if (backendResponse?.Success == true && backendResponse.Data != null)
                {
                    // Map backend response to frontend model
                    var result = new LoginResponseModel
                    {
                        Success = backendResponse.Success,
                        Message = backendResponse.Message,
                        Token = backendResponse.Data.Token,
                        UserId = (int)(backendResponse.Data.UserId ?? 0),
                        Username = backendResponse.Data.Username,
                        Role = backendResponse.Data.RoleName,
                        RoleId = (int)(backendResponse.Data.RoleId ?? 0),
                        BranchId = backendResponse.Data.BranchId
                    };
                    
                    Console.WriteLine($"AuthService: Mapped result - Success: {result.Success}, RoleId: {result.RoleId}");
                    return result;
                }
                
                return new LoginResponseModel { Success = false, Error = backendResponse?.Message ?? "Login failed" };
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
