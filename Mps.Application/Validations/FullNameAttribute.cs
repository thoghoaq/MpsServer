using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mps.Application.Validations
{
    public class FullNameAttribute : ValidationAttribute
    {
        // Regex pattern to include letters (A-Z, a-z), diacritics, and CJK (Chinese, Japanese, Korean) characters.
        private static readonly string FullNamePattern = @"^[\p{L}\p{M}]+([\p{L}\p{M} '-]*[\p{L}\p{M}]+)*$";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("The full name cannot be empty.");
            }

            Regex regex = new Regex(FullNamePattern, RegexOptions.Compiled);
            if (!regex.IsMatch(value.ToString()!))
            {
                return new ValidationResult("The full name is not in a valid format.");
            }

            return ValidationResult.Success;
        }
    }
}
