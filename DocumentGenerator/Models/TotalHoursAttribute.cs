using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class TotalHoursAttribute : ValidationAttribute
    {
        private readonly string month;

        public TotalHoursAttribute(string month)
        {
            this.month = month;
        }

        public bool IsWorkingDay(DateTime day)
        {
            var dayOfWeek = new DateTime(day.Year, day.Month, day.Day).DayOfWeek;
            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }

        int GetWorkingDays(DateTime mon)
        {
            int numberOfWorkinDays = 0;
            for (var day = mon; day.Month == mon.Month; day = day.AddDays(1))
                if (IsWorkingDay(day)) numberOfWorkinDays++;
            return numberOfWorkinDays;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var mon = validationContext.ObjectType.GetProperty(month);

            var workingMonth = (DateTime)mon.GetValue(validationContext.ObjectInstance);

            int numberOfWorkingDaysOfMonth = GetWorkingDays(workingMonth);

            var totalWorkingHours = (double)value;

            if (totalWorkingHours > 24 * numberOfWorkingDaysOfMonth)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }
    }
}