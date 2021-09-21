using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class EndWorkingDayAttribute : ValidationAttribute
    {
        private readonly string start;
        private readonly string hour;

        public EndWorkingDayAttribute(string start, string hour)
        {
            this.start = start;
            this.hour = hour;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDay = validationContext.ObjectType.GetProperty(start);
            var maxHour = validationContext.ObjectType.GetProperty(hour);

            var startWorkingDay = TimeSpan.Parse((string)startDay.GetValue(validationContext.ObjectInstance));
            var maxWorkingHours = TimeSpan.FromHours((double)maxHour.GetValue(validationContext.ObjectInstance));

            var endWorkingDay = TimeSpan.Parse((string)value);

            /*if (startWorkingDay.TotalMinutes % 30 != 0)
                return new ValidationResult();*/

            if (startWorkingDay + maxWorkingHours > endWorkingDay)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }

    }
}