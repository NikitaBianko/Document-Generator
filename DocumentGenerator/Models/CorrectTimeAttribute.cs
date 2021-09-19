using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class CorrectTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                TimeSpan time;

                if (value.GetType().Equals(typeof(double)))
                    time = TimeSpan.FromHours((double)value);
                else
                    time = TimeSpan.Parse((string)value);

                if(time.TotalHours > 0 && time.TotalHours < 24)
                    return ValidationResult.Success;
            }
            catch {
                return new ValidationResult(ErrorMessage);
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}