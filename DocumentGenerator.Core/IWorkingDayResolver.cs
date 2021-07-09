using System;
using System.Collections.Generic;

namespace DocumentGenerator.Core
{
    public interface IWorkingDayResolver
    {
        bool IsWorkingDay(DateTime day);
    }

    public class SimpleWorkingDaysResolver: IWorkingDayResolver
    {
        private readonly DateTime @from;
        private readonly DateTime to;
        private readonly List<DateTime> holidays;

        public SimpleWorkingDaysResolver(DateTime from, DateTime to, List<DateTime> holidays)
        {
            this.@from = @from;
            this.to = to;
            this.holidays = holidays ?? throw new ArgumentNullException(nameof(holidays));
        }

        public bool IsWorkingDay(DateTime day)
        {
            if (this.@from > day || day > this.to)
                throw new InvalidOperationException($"Can't resolve whether day {day} is working or not");
            
            var dayOfWeek = new DateTime(day.Year, day.Month, day.Day).DayOfWeek;
            return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday && !holidays.Contains(new DateTime(day.Year, day.Month, day.Day));
        }
    }
}