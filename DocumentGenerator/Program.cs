using HandlebarsDotNet;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentGenerator.Core;

namespace DocumentGenerator
{

    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("enter year/month");

            // string[] date = Console.ReadLine().Split('/');
            // DateTime month = new DateTime(int.Parse(date[0]), int.Parse(date[1]), 1);
            //
            // Console.WriteLine("The begining of the work day\nenter hours:minutes");
            //
            // string[] time = Console.ReadLine().Split(':');
            // TimeSpan startWorking = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);
            //
            // Console.WriteLine("number of working hours per month\nenter hours:minutes");
            //
            // time = Console.ReadLine().Split(':');
            // TimeSpan hours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);
            //
            // Console.WriteLine("minimum working hours\nenter hours:minutes");
            //
            // time = Console.ReadLine().Split(':');
            // TimeSpan minHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);
            //
            // Console.WriteLine("maximum working hours\nenter hours:minutes");
            //
            // time = Console.ReadLine().Split(':');
            // TimeSpan maxHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            string[] date = "2021/01".Split('/');
            DateTime month = new DateTime(int.Parse(date[0]), int.Parse(date[1]), 1);

            string[] time = "8:00".Split(':');
            TimeSpan startWorking = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            time = "19:00".Split(':');
            TimeSpan endWorking = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            time = "120:00".Split(':');
            TimeSpan hours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            time = "2:00".Split(':');
            TimeSpan minHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            time = "8:00".Split(':');
            TimeSpan maxHours = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);

            var pathToHolidaysFile = Path.Combine(Directory.GetCurrentDirectory(), "holidays.txt");
            var holidays = File.ReadAllLines(pathToHolidaysFile).Select(date => DateTime.Parse(date)).ToList();

            var pathToTemplateFile = Path.Combine(Directory.GetCurrentDirectory(), "index.html");
            var htmlDocumentTemplate = new HandlebarsDocumentTemplate(File.ReadAllText(pathToTemplateFile));
            var documentGenerator =
                new WorkingHoursDocumentGenerator(htmlDocumentTemplate);

            var htmlDocument = documentGenerator.Generate(new WorkingHoursParams(
                month.Year, month.Month,
                new WorkRequirements(hours, minHours, maxHours, startWorking, endWorking),
                new SimpleWorkingDaysResolver(DateTime.MinValue, DateTime.MaxValue, holidays)
            ));


            var pdfConverter = new PdfConverter();
            var pdfDocument = pdfConverter.Convert(htmlDocument);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "output.pdf"), pdfDocument.Content);
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "output.html"), htmlDocument.Content);

            Console.WriteLine("Done");
        }
    }
}
