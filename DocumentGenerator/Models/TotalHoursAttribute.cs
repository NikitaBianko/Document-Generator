using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class TotalHoursAttribute : ValidationAttribute
    {
        private readonly string numberDays;

        public TotalHoursAttribute(string numberDays)
        {
            this.numberDays = numberDays;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var workingDays = validationContext.ObjectType.GetProperty(numberDays);

            int numberOfWorkingDaysOfMonth = (int)workingDays.GetValue(validationContext.ObjectInstance);

            var totalWorkingHours = (double)value;

            if (totalWorkingHours > 24 * numberOfWorkingDaysOfMonth)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}