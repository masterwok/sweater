using System;
using System.ComponentModel.DataAnnotations;

namespace Sweater.Core.Attributes
{
    public class ValidEnumAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var enumType = value.GetType();
            var valid = Enum.IsDefined(enumType, value);

            return valid
                ? ValidationResult.Success
                : new ValidationResult($"{value} is not a valid value for type {enumType.Name}");
        }
    }
}