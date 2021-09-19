using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class TimeComparisonAttribute : ValidationAttribute
    {
        private readonly string hour;

        public TimeComparisonAttribute(string hour)
        {
            this.hour = hour;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minHour = validationContext.ObjectType.GetProperty(hour);

            TimeSpan maxWorkingHours;
            TimeSpan minWorkingHours;

            if(minHour.GetValue(validationContext.ObjectInstance).GetType().Equals(typeof(double)))
                minWorkingHours = TimeSpan.FromHours((double)minHour.GetValue(validationContext.ObjectInstance));
            else
                minWorkingHours = TimeSpan.Parse((string)minHour.GetValue(validationContext.ObjectInstance));

            if (value.GetType().Equals(typeof(double)))
                maxWorkingHours = TimeSpan.FromHours((double)value);
            else
                maxWorkingHours = TimeSpan.Parse((string)value);

            if (maxWorkingHours < minWorkingHours)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}