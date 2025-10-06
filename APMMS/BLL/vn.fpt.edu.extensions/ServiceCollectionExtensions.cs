using BLL.vn.fpt.edu.interfaces;
using BLL.vn.fpt.edu.services;
using DAL.vn.fpt.edu.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.vn.fpt.edu.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }

        public static IServiceCollection AddAuthDelegates(this IServiceCollection services)
        {
            services.AddScoped<Func<string, string, CancellationToken, Task<(bool Success, string? UserId, string? RoleName)>>>(sp =>
            {
                var userRepo = sp.GetRequiredService<IUserRepository>();  
                return (username, password, ct) => userRepo.VerifyCredentialsAsync(username, password, ct);
            });

            services.AddScoped<Func<string, CancellationToken, Task>>(sp =>
            {
                return (userId, ct) => Task.CompletedTask; // No-op for now
            });

            // JWT generator provided by BE layer
            return services;
        }
    }
}


