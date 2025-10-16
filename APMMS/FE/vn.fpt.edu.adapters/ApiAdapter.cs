using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace FE.vn.fpt.edu.adapters
{
    public class ApiAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiAdapter(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7000/api";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                // Log error
                return default(T);
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = $"{_baseUrl}/{endpoint}";
                Console.WriteLine($"ApiAdapter: Calling POST {url}");
                Console.WriteLine($"ApiAdapter: Request data: {json}");
                
                var response = await _httpClient.PostAsync(url, content);
                Console.WriteLine($"ApiAdapter: Response status: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ApiAdapter: Response content: {responseContent}");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ApiAdapter: Request failed with status {response.StatusCode}");
                    return default(T);
                }
                
                try
                {
                    var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine($"ApiAdapter: Deserialized result: {result}");
                    return result;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"ApiAdapter: JSON deserialization error: {ex.Message}");
                    Console.WriteLine($"ApiAdapter: Raw response: {responseContent}");
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiAdapter: Exception: {ex.Message}");
                return default(T);
            }
        }

        public async Task<bool> PutAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }
    }
}
