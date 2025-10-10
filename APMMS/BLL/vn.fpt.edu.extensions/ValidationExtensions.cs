using BLL.vn.fpt.edu.DTOs.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.vn.fpt.edu.extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginDto>, LoginRequestValidator>();
            services.AddScoped<IValidator<RegisterDto>, RegisterRequestValidator>();
            return services;
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Phone).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Phone));
        }
    }
}


