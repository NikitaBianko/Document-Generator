using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class MaxWorkingHoursAttribute : ValidationAttribute
    {
        private readonly string hours;
        private readonly string month;

        public MaxWorkingHoursAttribute(string hours, string month)
        {
            this.hours = hours;
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
            var totalHours = validationContext.ObjectType.GetProperty(hours);
            var mon = validationContext.ObjectType.GetProperty(month);

            var workingMonth = (DateTime)mon.GetValue(validationContext.ObjectInstance);
            var totalWorkingHours = (double)totalHours.GetValue(validationContext.ObjectInstance);

            int numberOfWorkingDaysOfMonth = GetWorkingDays(workingMonth);

            var maxHour = (double)value;

            if ((totalWorkingHours + numberOfWorkingDaysOfMonth - 1) / numberOfWorkingDaysOfMonth > maxHour)
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }

    }
}