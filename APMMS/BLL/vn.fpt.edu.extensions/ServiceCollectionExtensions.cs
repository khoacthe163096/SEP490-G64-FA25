using BLL.vn.fpt.edu.interfaces;
using BLL.vn.fpt.edu.services;
using DAL.vn.fpt.edu.interfaces;
using DAL.vn.fpt.edu.repository;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.vn.fpt.edu.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<JwtService>();
            
            // Vehicle Check-in services
            services.AddScoped<IVehicleCheckinService, VehicleCheckinService>();
            services.AddScoped<IVehicleCheckinRepository, VehicleCheckinRepository>();
            
            return services;
        }

        public static IServiceCollection AddAuthDelegates(this IServiceCollection services)
        {
            services.AddScoped<Func<string, string, CancellationToken, Task<(bool Success, string? UserId, string? RoleName, long? RoleId)>>>(sp =>
            {
                var userRepo = sp.GetRequiredService<IUserRepository>();  
                return (username, password, ct) => userRepo.VerifyCredentialsAsync(username, password, ct);
            });

            services.AddScoped<Func<string, CancellationToken, Task>>(sp =>
            {
                return (userId, ct) => Task.CompletedTask; // No-op for now
            });

            services.AddScoped<Func<string, string?, CancellationToken, Task<string>>>(sp =>
            {
                var jwtService = sp.GetRequiredService<JwtService>();
                return (userId, roleName, ct) => 
                {
                    // Parse role name to get role ID
                    var roleId = GetRoleIdFromRoleName(roleName);
                    return Task.FromResult(jwtService.GenerateToken(userId, roleName, roleId));
                };
            });
            
            // Add new delegate that can accept role ID directly
            services.AddScoped<Func<string, string?, int?, CancellationToken, Task<string>>>(sp =>
            {
                var jwtService = sp.GetRequiredService<JwtService>();
                return (userId, roleName, roleId, ct) => 
                {
                    return Task.FromResult(jwtService.GenerateToken(userId, roleName, roleId));
                };
            });

            return services;
        }
        
        private static int? GetRoleIdFromRoleName(string? roleName)
        {
            return roleName?.ToLower() switch
            {
                "admin" => 1,
                "branch manager" => 2,
                "accountant" => 3,
                "technician" => 4,
                "warehouse keeper" => 5,
                "consulter" => 6,
                "auto owner" => 7,
                "guest" => 8,
                _ => 7 // Default to Auto Owner
            };
        }
    }
}


