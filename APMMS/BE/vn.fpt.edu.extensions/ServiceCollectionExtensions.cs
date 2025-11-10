using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.repository;
using BE.vn.fpt.edu.services;
using BE.vn.fpt.edu.interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BE.vn.fpt.edu.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            // Add FluentValidation validators
            services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVehicleCheckinRepository, VehicleCheckinRepository>();
            services.AddScoped<IMaintenanceTicketRepository, MaintenanceTicketRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAutoOwnerRepository, AutoOwnerRepository>();
            services.AddScoped<ICarOfAutoOwnerRepository, CarOfAutoOwnerRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>(); // Not implemented yet
            services.AddScoped<IHistoryLogRepository, HistoryLogRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IServiceScheduleRepository, ServiceScheduleRepository>();
            services.AddScoped<IServiceTaskRepository, ServiceTaskRepository>();
            services.AddScoped<ITotalReceiptRepository, TotalReceiptRepository>();
            services.AddScoped<ITypeComponentRepository, TypeComponentRepository>();
            services.AddScoped<ITicketComponentRepository, TicketComponentRepository>();

            // Add Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IVehicleCheckinService, VehicleCheckinService>();
            services.AddScoped<IMaintenanceTicketService, MaintenanceTicketService>();
            // services.AddScoped<IComponentService, ComponentService>(); // Not implemented yet
            services.AddScoped<IServicePackageService, ServicePackageService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAutoOwnerService, AutoOwnerService>();
            services.AddScoped<ICarOfAutoOwnerService, CarOfAutoOwnerService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IHistoryLogService, HistoryLogService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IServiceScheduleService, ServiceScheduleService>();
            services.AddScoped<IServiceTaskService, ServiceTaskService>();
            services.AddScoped<ITotalReceiptService, TotalReceiptService>();
            services.AddScoped<ITypeComponentService, TypeComponentService>();
            services.AddScoped<ITicketComponentService, TicketComponentService>();
            services.AddScoped<JwtService>();

            return services;
        }
    }
}
