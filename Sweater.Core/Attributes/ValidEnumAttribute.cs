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
                : new ValidationResult(string.Format("{0} is not a valid value for type {1}", value, enumType.Name));
        }
    }
}