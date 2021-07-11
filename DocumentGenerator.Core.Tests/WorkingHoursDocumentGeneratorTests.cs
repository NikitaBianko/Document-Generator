using System;
using System.Linq;
using Xunit;

namespace DocumentGenerator.Core.Tests
{
    public class WorkingHoursDocumentGeneratorTests
    {
        [Fact]
        public void ItShouldCalculateWorkingHoursCorrectly()
        {
            var requirements = new WorkRequirements(
                TimeSpan.Parse("20:00"),
                TimeSpan.Parse("3:00"),
                TimeSpan.Parse("7:00"),
                TimeSpan.Parse("8:00"),
                TimeSpan.Parse("12:00")
            );
            
            var sut = new WorkingHoursCalculator(new WorkingHoursParams(2021, 7, requirements, new WeekendsWorkingDaysResolver()));

            var result = sut.Calculate();
            
            Assert.NotNull(result);

            var totalWorkingHours = result.Sum(x => x.DurationOfWork.TotalHours);

            Assert.Equal(totalWorkingHours, requirements.TotalMonthlyHours.TotalHours);

            foreach (var day in result)
            {
                Assert.True(day.BeginningOfWork >= requirements.MinDailyWorkingHours);
                Assert.True(day.EndOfWork <= requirements.MaxWorkingDayEnd);
            }
        }
    }

    class WeekendsWorkingDaysResolver : IWorkingDayResolver
    {
        public bool IsWorkingDay(DateTime day)
        {
            return day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}