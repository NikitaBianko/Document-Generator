using System;
using System.Collections.Generic;

namespace Document_Generator
{
    class WorkingHours
    {
        public DateTime Day { get; set; }
        public TimeSpan BeginningOfWork { get; set; }
        public TimeSpan DurationOfWork { get; set; }
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
