using HandlebarsDotNet;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Document_Generator
{
    class Program
    {
        static void Main(string[] args) 
        {
            Console.WriteLine("enter year/month");

            string[] date = Console.ReadLine().Split('/');
            DateTime month = new DateTime(int.Parse(date[0]), int.Parse(date[1]), 1);

            Console.WriteLine("The begining of the work day\nenter hours:minutes");

            string[] time = Console.ReadLine().Split(':');
            TimeSpan startWorking = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            Console.WriteLine("number of working hours per month\nenter hours:minutes");

            time = Console.ReadLine().Split(':');
            TimeSpan hours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            Console.WriteLine("minimum working hours\nenter hours:minutes");

            time = Console.ReadLine().Split(':');
            TimeSpan minHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            Console.WriteLine("maximum working hours\nenter hours:minutes");

            time = Console.ReadLine().Split(':');
            TimeSpan maxHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            List<WorkingHours> document = DocumentGenerator(new WorkingMonth(month, hours, minHours, maxHours, startWorking));

            Console.WriteLine("Datum beginn Ende Arbeitszeit");

            TimeSpan CheckTime = new TimeSpan(0, 0, 0);

            foreach (var item in document)
            {
                Console.WriteLine(item);
                CheckTime += item.DurationOfWork;
            }
            Console.WriteLine($"total hours -> {CheckTime.TotalHours}\n{document.Count} days");
            PDFGenerate(document);
        }
        public static void PDFGenerate(List<WorkingHours> document)
        {
            Handlebars.RegisterHelper("date", (writer, context, parameters) =>
            {
                DateTime date = (DateTime)context["Day"];
                writer.WriteSafeString(date.ToString(@"dd\.mm\.yyyy", new CultureInfo("de-De")));
            });

            Handlebars.RegisterHelper("begin", (writer, context, parameters) =>
            {
                TimeSpan BeginningOfWork = (TimeSpan)context["BeginningOfWork"];
                writer.WriteSafeString(BeginningOfWork.ToString(@"hh\:mm", new CultureInfo("en-US")));
            });

            Handlebars.RegisterHelper("ende", (writer, context, parameters) =>
            {
                TimeSpan BeginningOfWork = (TimeSpan)context["BeginningOfWork"];
                TimeSpan durationWork = (TimeSpan)context["DurationOfWork"];
                writer.WriteSafeString((BeginningOfWork + durationWork).ToString(@"hh\:mm", new CultureInfo("en-US")));
            });

            Handlebars.RegisterHelper("durationWork", (writer, context, parameters) =>
            {
                TimeSpan durationWork = (TimeSpan)context["DurationOfWork"];
                writer.WriteSafeString(durationWork.ToString(@"hh\:mm", new CultureInfo("en-US")));
            });

            string html = System.IO.File.ReadAllText(@"D:\nikita\projects\DocumentGenerator\DocumentGenerator\index.html");

            var template = Handlebars.Compile(html);

            var data = new
            {
                WorkingHours = document
            };

            var result = template(data);

            var htmlToPdf = new HtmlToPDFCore.HtmlToPDF();
            var pdf = htmlToPdf.ReturnPDF(result);

            File.Delete(@"D:\nikita\projects\DocumentGenerator\index.pdf");

            FileStream fs = new FileStream(@"D:\nikita\projects\DocumentGenerator\index.pdf", FileMode.CreateNew);
            fs.Write(pdf, 0, pdf.Length);
            fs.Close();
        }
        public static List<WorkingHours> DocumentGenerator(WorkingMonth requirements, List<DateTime> holidays = null)
        {
            List<WorkingHours> document = new List<WorkingHours>(); 

            List<DateTime> workingDays = GetWorkingDays(requirements, holidays);

            if (requirements.WorkingHours / workingDays.Count > requirements.MaximumWorkingHours)
                throw new ArgumentException("average operating time is greater than maximum");

            workingDays = deletingWorkingDays(requirements, workingDays);

            TimeSpan average = Rounding(requirements.WorkingHours / workingDays.Count);
            TimeSpan remainder = requirements.WorkingHours - average * workingDays.Count;

            List<TimeSpan> workingHours = new List<TimeSpan>(workingDays.Count);

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
            if (average == requirements.MinimumWorkingHours && average.TotalMinutes / 30 < 3) repeat = 0;

            for (int i = 0; i < 1; i++)
                workingHours = Noise(requirements, workingHours);

            for (int it = 0; it < workingDays.Count; it++)
                document.Add(new WorkingHours(workingDays[it], requirements.StartWorking, workingHours[it]));

            return document;
        }
        public static List<DateTime> deletingWorkingDays(WorkingMonth requirements, List<DateTime> workingDays)
        {

            int maximumNumberDays = (int)Math.Min(Math.Floor(requirements.WorkingHours / requirements.MinimumWorkingHours), workingDays.Count);
            int minimumNumberDays = (int)Math.Ceiling(requirements.WorkingHours / requirements.MaximumWorkingHours);

            Random rnd = new Random();

            int numberWorkingDays = rnd.Next(minimumNumberDays, maximumNumberDays);

            for (int i = 0; workingDays.Count != numberWorkingDays; i++)
            {
                int x = rnd.Next(0, workingDays.Count);
                workingDays.RemoveAt(x);
            }


            return workingDays;
        }
        public static List<TimeSpan> Noise(WorkingMonth requirements, List<TimeSpan> workingHours)
        {
            for (int i = 0; i < workingHours.Count && workingHours.Count > 1; i++)
            {
                Random rnd = new Random();

                var x = rnd.Next(0, workingHours.Count);
                var y = rnd.Next(0, workingHours.Count);
                while (x == y) y = rnd.Next(0, workingHours.Count);

                int minimumRangeFirstPoint = (int)(Math.Min(Math.Abs((workingHours[x] - requirements.MinimumWorkingHours).TotalMinutes), Math.Abs((requirements.MaximumWorkingHours - workingHours[x]).TotalMinutes))) / 30;
                int minimumRangeSecondPoint = (int)(Math.Min(Math.Abs((workingHours[y] - requirements.MinimumWorkingHours).TotalMinutes), Math.Abs((requirements.MaximumWorkingHours - workingHours[y]).TotalMinutes))) / 30;

                int minimumRange = Math.Min(minimumRangeFirstPoint, minimumRangeSecondPoint);

                if (minimumRange == 0)
                {
                    int countMax = 0;
                    int countMin = 0;
                    foreach (var time in workingHours)
                    {
                        if (time == requirements.MaximumWorkingHours) countMax++;
                        if (time == requirements.MinimumWorkingHours) countMin++;
                    }
                    if(countMax >= workingHours.Count - 2 || countMin >= workingHours.Count - 2)
                        return workingHours;
                    i--;
                    continue;
                }

                int change = rnd.Next(1, minimumRange);

                workingHours[x] += TimeSpan.FromMinutes(change * 30);
                workingHours[y] -= TimeSpan.FromMinutes(change * 30);

                workingHours = Smoothing(workingHours, requirements);
            }

            return workingHours;
        }
        public static List<TimeSpan> Smoothing(List<TimeSpan> time, WorkingMonth requirements)
        {
            Random rnd = new Random();

            for (int i = 0, j = time.Count - 1; i < time.Count - 1 && j > 0; i++, j--)
            {
                if(time[i] >= requirements.MinimumWorkingHours && time[i] < requirements.MaximumWorkingHours && time[i + 1] > requirements.MinimumWorkingHours && time[i + 1] <= requirements.MaximumWorkingHours)
                    if(time[i + 1] - time[i] > new TimeSpan(0, 30, 0) && rnd.Next(0, 2) != 0)
                    {
                        time[i] += new TimeSpan(0, 30, 0);
                        time[i + 1] -= new TimeSpan(0, 30, 0);
                    }
                if (time[j] >= requirements.MinimumWorkingHours && time[j] < requirements.MaximumWorkingHours && time[j - 1] > requirements.MinimumWorkingHours && time[j - 1] <= requirements.MaximumWorkingHours)
                    if (time[j - 1] - time[j] > new TimeSpan(0, 30, 0) && rnd.Next(0, 2) != 0)
                    {
                        time[j] += new TimeSpan(0, 30, 0);
                        time[j - 1] -= new TimeSpan(0, 30, 0);
                    }
            }

            return time;

        }
        public static TimeSpan Rounding(TimeSpan time)
        {
            int numberOfHalfHours = (int)(time.TotalMinutes / 30);

            if (time.TotalMinutes - numberOfHalfHours * 30 >= 15)
                return TimeSpan.FromMinutes((numberOfHalfHours + 1) * 30);
            else
                return TimeSpan.FromMinutes(numberOfHalfHours * 30);
        }
        public static bool CheckWorkingDay(DateTime date, List<DateTime> holidays = null)
        {
            var DayOfWeek = new DateTime(date.Year, date.Month, date.Day).DayOfWeek;
            return DayOfWeek != DayOfWeek.Saturday && DayOfWeek != DayOfWeek.Sunday && (holidays == null || holidays.Contains(new DateTime(date.Year, date.Month, date.Day)));
        }
        public static List<DateTime> GetWorkingDays(WorkingMonth requirements, List<DateTime> holidays = null)
        {
            var workingDay = new List<DateTime>();

            for (int day = 1; day <= DateTime.DaysInMonth(requirements.Date.Year, requirements.Date.Month); day++)
                if (CheckWorkingDay(new DateTime(requirements.Date.Year, requirements.Date.Month, day), holidays))
                    workingDay.Add(new DateTime(requirements.Date.Year, requirements.Date.Month, day));

            return workingDay;
        }
    }
}
