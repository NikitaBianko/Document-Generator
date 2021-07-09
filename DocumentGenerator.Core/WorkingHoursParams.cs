using System;

namespace DocumentGenerator.Core
{
    public class WorkingHoursParams
    {
        public int Year { get; }
        public int Month { get; }
        public WorkRequirements Requirements { get; }
        public IWorkingDayResolver WorkingDayResolver { get; }

        public WorkingHoursParams(
            int year, 
            int month, 
            WorkRequirements requirements, 
            IWorkingDayResolver workingDayResolver)
        {
            Year = year;
            Month = month;
            Requirements = requirements;
            WorkingDayResolver = workingDayResolver;
        }
    }

    public class WorkRequirements
    {
        public TimeSpan TotalMonthlyHours { get; }
        public TimeSpan MinDailyWorkingHours { get; }
        public TimeSpan MaxDailyWorkingHours { get; }
        public TimeSpan MinWorkingDayStart { get; }
        public TimeSpan MaxWorkingDayEnd { get; }

        public WorkRequirements(
            TimeSpan totalMonthlyHours, 
            TimeSpan minDailyWorkingHours, 
            TimeSpan maxDailyWorkingHours, 
            TimeSpan minWorkingDayStart, 
            TimeSpan maxWorkingDayEnd)
        {
            TotalMonthlyHours = totalMonthlyHours;
            MinDailyWorkingHours = minDailyWorkingHours;
            MaxDailyWorkingHours = maxDailyWorkingHours;
            MinWorkingDayStart = minWorkingDayStart;
            MaxWorkingDayEnd = maxWorkingDayEnd;
        }
    }
}