using System;
using System.Collections.Generic;

namespace DocumentGenerator.Core
{
    public class WorkingHoursCalculator
    {
        private readonly WorkingHoursParams @params;

        public WorkingHoursCalculator(WorkingHoursParams @params)
        {
            this.@params = @params;
        }
        
        public List<WorkingHours> Calculate()
        {
            var document = new List<WorkingHours>();

            var workingDays = GetWorkingDays(@params.Year, @params.Month);
            var requirements = @params.Requirements;
            
            if (requirements.TotalMonthlyHours.Ticks > requirements.MaxDailyWorkingHours.Ticks * workingDays.Count)
                throw new ArgumentException("average operating time is greater than maximum");

            workingDays = DeletingWorkingDays(requirements, workingDays);

            var average = Rounding(requirements.TotalMonthlyHours / workingDays.Count);
            var remainder = requirements.TotalMonthlyHours - average * workingDays.Count;

            var workingHours = new List<TimeSpan>(workingDays.Count);

            for (int day = 0; day < workingDays.Count; day++)
            {
                workingHours.Add(average);
                if(remainder.TotalMinutes != 0)
                    if (remainder.TotalMinutes > 0)
                    {
                        workingHours[day] += new TimeSpan(0, 30, 0);
                        remainder -= new TimeSpan(0, 30, 0);
                    }
                    else
                    {
                        workingHours[day] -= new TimeSpan(0, 30, 0);
                        remainder += new TimeSpan(0, 30, 0);
                    }
            }

            Random rnd = new Random();
            int repeat = rnd.Next(1, (int)(Math.Ceiling((double)workingDays.Count / 10)));
            if (average == requirements.MinDailyWorkingHours && average.TotalMinutes / 30 < 3) repeat = 0;

            for (int i = 0; i < repeat; i++)
                workingHours = Noise(workingHours, requirements.MinDailyWorkingHours, requirements.MaxDailyWorkingHours);

            for (int it = 0; it < workingDays.Count; it++)
                document.Add(new WorkingHours(workingDays[it], requirements.MinWorkingDayStart, workingHours[it]));

            return document;
        }

        public static List<DateTime> DeletingWorkingDays(WorkRequirements requirements, List<DateTime> workingDays)
        {

            int maximumNumberDays = (int)Math.Min(Math.Floor(requirements.TotalMonthlyHours / requirements.MinDailyWorkingHours), workingDays.Count);
            int minimumNumberDays = (int)Math.Ceiling(requirements.TotalMonthlyHours / requirements.MaxDailyWorkingHours);

            Random rnd = new Random();

            int numberWorkingDays = rnd.Next(minimumNumberDays, maximumNumberDays);

            for (int i = 0; workingDays.Count != numberWorkingDays; i++)
            {
                int x = rnd.Next(0, workingDays.Count);
                workingDays.RemoveAt(x);
            }


            return workingDays;
        }
  
        public static List<TimeSpan> Noise(List<TimeSpan> days, TimeSpan minTime, TimeSpan maxTime)
        {
            for (int i = 0; i < days.Count && days.Count > 1; i++)
            {
                Random rnd = new Random();

                var x = rnd.Next(0, days.Count);
                var y = rnd.Next(0, days.Count);
                while (x == y) y = rnd.Next(0, days.Count);

                int minimumRangeFirstPoint = (int)(Math.Min(Math.Abs((days[x] - minTime).TotalMinutes), Math.Abs((maxTime - days[x]).TotalMinutes))) / 30;
                int minimumRangeSecondPoint = (int)(Math.Min(Math.Abs((days[y] - minTime).TotalMinutes), Math.Abs((maxTime - days[y]).TotalMinutes))) / 30;

                int minimumRange = Math.Min(minimumRangeFirstPoint, minimumRangeSecondPoint);

                if (minimumRange == 0)
                {
                    int countMax = 0;
                    int countMin = 0;
                    foreach (var time in days)
                    {
                        if (time == maxTime) countMax++;
                        if (time == minTime) countMin++;
                    }
                    if(countMax >= days.Count - 2 || countMin >= days.Count - 2)
                        return days;
                    i--;
                    continue;
                }

                int change = rnd.Next(1, minimumRange);

                days[x] += TimeSpan.FromMinutes(change * 30);
                days[y] -= TimeSpan.FromMinutes(change * 30);

                days = Smoothing(days, minTime, maxTime);
            }

            return days;
        }
        
        private static List<TimeSpan> Smoothing(List<TimeSpan> time, TimeSpan minTime, TimeSpan maxTime)
        {
            Random rnd = new Random();

            for (int i = 0, j = time.Count - 1; i < time.Count - 1 && j > 0; i++, j--)
            {
                if(time[i] >= minTime && time[i] < maxTime && time[i + 1] > minTime && time[i + 1] <= maxTime)
                    if(time[i + 1] - time[i] > new TimeSpan(0, 30, 0) && rnd.Next(0, 2) != 0)
                    {
                        time[i] += new TimeSpan(0, 30, 0);
                        time[i + 1] -= new TimeSpan(0, 30, 0);
                    }
                if (time[j] >= minTime && time[j] < maxTime && time[j - 1] > minTime && time[j - 1] <= maxTime)
                    if (time[j - 1] - time[j] > new TimeSpan(0, 30, 0) && rnd.Next(0, 2) != 0)
                    {
                        time[j] += new TimeSpan(0, 30, 0);
                        time[j - 1] -= new TimeSpan(0, 30, 0);
                    }
            }

            return time;

        }
        
        private static TimeSpan Rounding(TimeSpan time)
        {
            int numberOfHalfHours = (int)(time.TotalMinutes / 30);

            if (time.TotalMinutes - numberOfHalfHours * 30 >= 15)
                return TimeSpan.FromMinutes((numberOfHalfHours + 1) * 30);
            else
                return TimeSpan.FromMinutes(numberOfHalfHours * 30);
        }
        
        private List<DateTime> GetWorkingDays(int year, int month)
        {
            var workingDay = new List<DateTime>();

            for (var day = new DateTime(year, month, 1); day.Month == month; day = day.AddDays(1))
                if (this.@params.WorkingDayResolver.IsWorkingDay(day))
                    workingDay.Add(day);

            return workingDay;
        }
        
    }
}