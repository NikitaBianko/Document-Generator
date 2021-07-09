using System;

namespace DocumentGenerator.Core
{
    public class WorkingHours
    {
        public DateTime Day { get; }
        public TimeSpan BeginningOfWork { get; }
        public TimeSpan DurationOfWork { get; }
        public TimeSpan EndOfWork => this.BeginningOfWork + this.DurationOfWork;
        
        public WorkingHours(DateTime day, TimeSpan beginningOfWork, TimeSpan durationOfWork)
        {
            Day = day;
            BeginningOfWork = beginningOfWork;
            DurationOfWork = durationOfWork;
        }

        public override string ToString()
        {
            return $"{Day.Day}.{Day.Month}.{Day.Year} {BeginningOfWork} {BeginningOfWork + DurationOfWork} {DurationOfWork}";
        }
    }
}
