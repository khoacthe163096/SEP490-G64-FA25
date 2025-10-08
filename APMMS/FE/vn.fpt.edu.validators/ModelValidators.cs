using System.ComponentModel.DataAnnotations;
using FE.vn.fpt.edu.models;

namespace FE.vn.fpt.edu.validators
{
    public static class ModelValidators
    {
        public static ValidationResult? ValidateUserModel(UserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
                return new ValidationResult("Username is required");

            if (string.IsNullOrWhiteSpace(model.Email))
                return new ValidationResult("Email is required");

            if (!IsValidEmail(model.Email))
                return new ValidationResult("Invalid email format");

            if (string.IsNullOrWhiteSpace(model.Phone))
                return new ValidationResult("Phone is required");

            return ValidationResult.Success;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class UserModelValidator : IValidatableObject
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username))
                yield return new ValidationResult("Username is required", new[] { nameof(Username) });

            if (string.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult("Email is required", new[] { nameof(Email) });

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
                yield return new ValidationResult("Invalid email format", new[] { nameof(Email) });

            if (string.IsNullOrWhiteSpace(Phone))
                yield return new ValidationResult("Phone is required", new[] { nameof(Phone) });
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
