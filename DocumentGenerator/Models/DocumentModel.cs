using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentGenerator.Models
{
    public class DocumentModel
    {
        [Required(ErrorMessage = "Field is empty.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Field is empty.")]
        public DateTime workingMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        [Required(ErrorMessage = "Field is empty.")]
        [DivisibilityTime(ErrorMessage = "Time must be divisible by 30 minutes.")]
        [TotalHours("numberOfWorkingDaysOfMonth", ErrorMessage = "Working hours per day exceed 24 hours.")]
        public double totalHours { get; set; }
        [Required(ErrorMessage = "Field is empty")]
        [CorrectTime(ErrorMessage = "Incorrect time.")]
        [DivisibilityTime(ErrorMessage = "Time must be divisible by 30 minutes.")]
        public double minWorkingHours { get; set; }
        [Required(ErrorMessage = "Field is empty.")]
        [CorrectTime(ErrorMessage = "Incorrect time.")]
        [DivisibilityTime(ErrorMessage = "Time must be divisible by 30 minutes.")]
        [TimeComparison("minWorkingHours", ErrorMessage = "Maximum running time is less than minimum.")]
        [MaxWorkingHours("totalHours", "numberOfWorkingDaysOfMonth", ErrorMessage = "Average operating time is greater than maximum.")]
        public double maxWorkingHours { get; set; }
        [Required(ErrorMessage = "Field is empty")]
        [CorrectTime(ErrorMessage = "Incorrect time.")]
        [DivisibilityTime(ErrorMessage = "Time must be divisible by 30 minutes.")]
        public string startWorkingDay { get; set; }
        [Required(ErrorMessage = "Field is empty.")]
        [CorrectTime(ErrorMessage = "Incorrect time.")]
        [DivisibilityTime(ErrorMessage = "Time must be divisible by 30 minutes.")]
        [EndWorkingDay("startWorkingDay", "maxWorkingHours", ErrorMessage = "The period from the beginning of the working day to the end of the working day is less than the maximum working time.")]
        [TimeComparison("startWorkingDay", ErrorMessage = "The start time of the working day is longer than the end time.")]
        public string endWorkingDay { get; set; }
        [Required(ErrorMessage = "Field is empty.")]
        public TypeOfFile? typeOfFile { get; set; } = TypeOfFile.pdf;

        public int numberOfWorkingDaysOfMonth => (this.workingMonth != null ? GetWorkingDays() : 0);
        public List<DateTime> holidays { get; set; }
        public bool IsWorkingDay(DateTime day)
        {
            var dayOfWeek = new DateTime(day.Year, day.Month, day.Day).DayOfWeek;
            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday && !holidays.Contains(new DateTime(day.Year, day.Month, day.Day));
        }
        private int GetWorkingDays()
        {
            int numberOfWorkinDays = 0;
            for (var day = workingMonth; day.Month == workingMonth.Month; day = day.AddDays(1))
                if (IsWorkingDay(day)) numberOfWorkinDays++;
            return numberOfWorkinDays;
        }
    }
}
