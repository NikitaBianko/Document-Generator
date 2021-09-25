using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class MaxWorkingHoursAttribute : ValidationAttribute
    {
        private readonly string hours;
        private readonly string numberDays;

        public MaxWorkingHoursAttribute(string hours, string numberDays)
        {
            this.hours = hours;
            this.numberDays = numberDays;
        }

        double Rounding(double n)
        {
            int m = (int)(n / 0.5);

            if (n - (m * 0.5) >= 0.5) return m + 0.5;
            else return m * 0.5;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var totalHours = validationContext.ObjectType.GetProperty(hours);
            var workingDays = validationContext.ObjectType.GetProperty(numberDays);

            int numberOfWorkingDaysOfMonth = (int)workingDays.GetValue(validationContext.ObjectInstance);
            var totalWorkingHours = (double)totalHours.GetValue(validationContext.ObjectInstance);

            var maxHour = (double)value;

            if (Rounding(totalWorkingHours / numberOfWorkingDaysOfMonth) >= maxHour)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }

    }
}