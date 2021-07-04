using System;

namespace Document_Generator
{
    class WorkingMonth
    {
        public DateTime Date { get; set; }
        public TimeSpan WorkingHours { get; set; }
        public TimeSpan MinimumWorkingHours { get; set; }
        public TimeSpan MaximumWorkingHours { get; set; }
        public TimeSpan StartWorking { get; set; }
        public WorkingMonth(DateTime date, TimeSpan workingHours, TimeSpan minimumWorkingHours, TimeSpan maximumWorkingHours, TimeSpan startWorking)
        {
            Date = date;
            WorkingHours = workingHours;
            MinimumWorkingHours = minimumWorkingHours;
            MaximumWorkingHours = maximumWorkingHours;
            StartWorking = startWorking;
        }
    }
}
