using FE.vn.fpt.edu.adapters;
using FE.vn.fpt.edu.services;

namespace FE.vn.fpt.edu.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrontendServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add HttpContextAccessor
            services.AddHttpContextAccessor();

            // Add HttpClient
            services.AddHttpClient<ApiAdapter>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7000/api");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Add Adapters
            services.AddScoped<ApiAdapter>();

            // Add Services
            services.AddScoped<UserService>();
            services.AddScoped<AuthService>();
            services.AddScoped<VehicleCheckinService>();
            services.AddScoped<MaintenanceTicketService>();
            services.AddScoped<ComponentService>();
            services.AddScoped<TypeComponentService>();

            return services;
        }
    }
}
