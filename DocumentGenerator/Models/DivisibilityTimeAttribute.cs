using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class DivisibilityTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value.GetType().Equals(typeof(double)))
            {
                if((double)value % 0.5 != 0)
                    return new ValidationResult(ErrorMessage);
                return ValidationResult.Success;
            }

            var hour = TimeSpan.Parse((string)value);

            if (hour.TotalHours % 0.5 != 0)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}