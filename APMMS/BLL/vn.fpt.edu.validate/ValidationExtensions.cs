using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.DTOs.AutoOwner;
using BLL.vn.fpt.edu.DTOs.Component;
using BLL.vn.fpt.edu.DTOs.Employee;
using BLL.vn.fpt.edu.DTOs.Feedback;
using BLL.vn.fpt.edu.DTOs.HistoryLog;
using BLL.vn.fpt.edu.DTOs.MaintenanceTicket;
using BLL.vn.fpt.edu.DTOs.Profile;
using BLL.vn.fpt.edu.DTOs.Report;
using BLL.vn.fpt.edu.DTOs.Role;
using BLL.vn.fpt.edu.DTOs.ServicePackage;
using BLL.vn.fpt.edu.DTOs.ServiceSchedule;
using BLL.vn.fpt.edu.DTOs.ServiceTask;
using BLL.vn.fpt.edu.DTOs.TotalReceipt;
using BLL.vn.fpt.edu.DTOs.TypeComponent;
using BLL.vn.fpt.edu.DTOs.VehicleCheckin;

namespace BLL.vn.fpt.edu.validate
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            // Auth validators
            services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
            services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();

            // AutoOwner validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.AutoOwner.RequestDto>, AutoOwnerCreateRequestValidator>();

            // Component validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Component.RequestDto>, ComponentCreateRequestValidator>();

            // Employee validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Employee.RequestDto>, EmployeeCreateRequestValidator>();

            // Feedback validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Feedback.CreateRequestDto>, FeedbackCreateRequestValidator>();

            // HistoryLog validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.HistoryLog.RequestDto>, HistoryLogCreateRequestValidator>();

            // MaintenanceTicket validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.MaintenanceTicket.RequestDto>, MaintenanceTicketCreateRequestValidator>();

            // Profile validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Profile.RequestDto>, ProfileCreateRequestValidator>();

            // Report validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Report.RequestDto>, ReportCreateRequestValidator>();

            // Role validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.Role.RequestDto>, RoleCreateRequestValidator>();

            // ServicePackage validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.ServicePackage.RequestDto>, ServicePackageCreateRequestValidator>();

            // ServiceSchedule validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.ServiceSchedule.RequestDto>, ServiceScheduleCreateRequestValidator>();

            // ServiceTask validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.ServiceTask.RequestDto>, ServiceTaskCreateRequestValidator>();

            // TotalReceipt validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.TotalReceipt.RequestDto>, TotalReceiptCreateRequestValidator>();

            // TypeComponent validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.TypeComponent.RequestDto>, TypeComponentCreateRequestValidator>();

            // VehicleCheckin validators
            services.AddScoped<IValidator<BLL.vn.fpt.edu.DTOs.VehicleCheckin.RequestDto>, VehicleCheckinCreateRequestValidator>();

            return services;
        }
    }

    // Auth Validators
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Valid email is required");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required");
        }
    }

    // Placeholder validators for other DTOs
    public class AutoOwnerCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.AutoOwner.RequestDto> { }
    public class ComponentCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Component.RequestDto> { }
    public class EmployeeCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Employee.RequestDto> { }
    public class FeedbackCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Feedback.CreateRequestDto> { }
    public class HistoryLogCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.HistoryLog.RequestDto> { }
    public class MaintenanceTicketCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.MaintenanceTicket.RequestDto> { }
    public class ProfileCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Profile.RequestDto> { }
    public class ReportCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Report.RequestDto> { }
    public class RoleCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.Role.RequestDto> { }
    public class ServicePackageCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.ServicePackage.RequestDto> { }
    public class ServiceScheduleCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.ServiceSchedule.RequestDto> { }
    public class ServiceTaskCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.ServiceTask.RequestDto> { }
    public class TotalReceiptCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.TotalReceipt.RequestDto> { }
    public class TypeComponentCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.TypeComponent.RequestDto> { }
    public class VehicleCheckinCreateRequestValidator : AbstractValidator<BLL.vn.fpt.edu.DTOs.VehicleCheckin.RequestDto> { }
}


