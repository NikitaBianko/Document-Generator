using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentGenerator.Core;
using DocumentGenerator.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DocumentGenerator.Controller
{
    [ApiController]
    [Route("document")]
    public class DocumentsController : ControllerBase
    {
        private string ValidateName(string name)
        {
            if (name == null)
                ModelState.AddModelError("name", "Invalid field");
            return name;
        }
        private DateTime ValidateMonth(string month)
        {
            try
            {
                return DateTime.Parse(month);
            }
            catch
            {
                ModelState.AddModelError("month", "Invalid field");
            }
            return new DateTime();
        }
        private TimeSpan ValidateTotalHours(string totalHours)
        {
            try
            {
                var hours = TimeSpan.FromHours(double.Parse(totalHours));

                if (hours.TotalHours % 0.5 != 0)
                    ModelState.AddModelError("totalHours", "Time must be divisible by 30 minutes");

                return hours;

            }
            catch
            {
                ModelState.AddModelError("totalHours", "Invalid field");
            }
            return new TimeSpan();
        }
        private TimeSpan ValidateMinWorkingHours(string minWorkingHours)
        {
            try
            {
                var minHours = TimeSpan.FromHours(double.Parse(minWorkingHours));

                if (minHours.TotalHours % 0.5 != 0)
                    ModelState.AddModelError("minWorkingHours", "Time must be divisible by 30 minutes");

                if (minHours.TotalHours >= 24 || minHours.TotalHours <= 0)
                    ModelState.AddModelError("minWorkingHours", "Incorrect time");

                return minHours;
            }
            catch
            {
                ModelState.AddModelError("minWorkingHours", "Invalid field");
            }

            return new TimeSpan();
        }
        private TimeSpan ValidateMaxWorkingHours(string maxWorkingHours, TimeSpan minWorkingHours)
        {
            try
            {
                TimeSpan maxHours = TimeSpan.FromHours(double.Parse(maxWorkingHours));

                if (minWorkingHours != null && minWorkingHours > maxHours)
                    ModelState.AddModelError("maxWorkingHours", "Maximum running time is less than minimum");

                if (maxHours.TotalHours % 0.5 != 0)
                    ModelState.AddModelError("maxWorkingHours", "Time must be divisible by 30 minutes");

                if (maxHours.TotalHours >= 24 || maxHours.TotalHours <= 0)
                    ModelState.AddModelError("maxWorkingHours", "Incorrect time");

                return maxHours;
            }
            catch
            {
                ModelState.AddModelError("maxWorkingHours", "Invalid field");
            }

            return new TimeSpan();
        }
        private TimeSpan ValidateStartWorkingDay(string startWorkingDay)
        {
            try
            {
                var startWorking = TimeSpan.Parse(startWorkingDay);

                if(startWorking.TotalHours % 0.5 != 0)
                    ModelState.AddModelError("startWorkingDay", "Time must be divisible by 30 minutes");

                if (startWorking.TotalHours >= 24 || startWorking.TotalHours <= 0)
                    ModelState.AddModelError("startWorkingDay", "Incorrect time");

                return startWorking;
            }
            catch
            {
                ModelState.AddModelError("startWorkingDay", "Invalid field");
            }
            return new TimeSpan();
        }
        private TimeSpan ValidateEndWorkingDay(string endWorkingDay, TimeSpan startWorkingDay, TimeSpan maxWorkingHours)
        {
            try
            {
                var endWorking = TimeSpan.Parse(endWorkingDay);

                if (startWorkingDay != null && startWorkingDay > endWorking)
                    ModelState.AddModelError("endWorkingDay", "Work end time is less than start time");

                if (startWorkingDay != null && maxWorkingHours != null && startWorkingDay + maxWorkingHours > endWorking)
                    ModelState.AddModelError("endWorkingDay", "The period from the beginning of the working day to the end of the working day is less than the maximum working time");

                if (endWorking.TotalHours % 0.5 != 0)
                    ModelState.AddModelError("endWorkingDay", "Time must be divisible by 30 minutes");

                if (endWorking.TotalHours >= 24 || endWorking.TotalHours <= 0)
                    ModelState.AddModelError("endWorkingDay", "Incorrect time");

                return endWorking;
            }
            catch
            {
                ModelState.AddModelError("endWorkingDay", "Invalid field");
            }
            

            return new TimeSpan();
        }

        [HttpGet]
        public IActionResult Get(string name, string workingMonth, string totalHours, string minWorkingHours, string maxWorkingHours, string endWorkingDay, string startWorkingDay, string typeOfFile)
        {
            try
            {
                ValidateName(name);

                var month = ValidateMonth(workingMonth);

                var hours = ValidateTotalHours(totalHours);

                var minHours = ValidateMinWorkingHours(minWorkingHours);

                var maxHours = ValidateMaxWorkingHours(maxWorkingHours, minHours);

                var startWorking = ValidateStartWorkingDay(startWorkingDay);

                var endWorking = ValidateEndWorkingDay(endWorkingDay, startWorking, maxHours);

                if (typeOfFile != "pdf" && typeOfFile != "html")
                    ModelState.AddModelError("typeOfFile", "Document type not defined");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pathToHolidaysFile = Path.Combine(Directory.GetCurrentDirectory(), "holidays.txt");
                var holidays = System.IO.File.ReadAllLines(pathToHolidaysFile).Select(date => DateTime.Parse(date)).ToList();

                var pathToTemplateFile = Path.Combine(Directory.GetCurrentDirectory(), "index.html");
                var htmlDocumentTemplate = new HandlebarsDocumentTemplate(System.IO.File.ReadAllText(pathToTemplateFile));

                var documentGenerator =
                    new WorkingHoursDocumentGenerator(htmlDocumentTemplate);

                var htmlDocument = documentGenerator.Generate(new WorkingHoursParams(
                    month.Year, month.Month,
                    new WorkRequirements(hours, minHours, maxHours, startWorking, endWorking),
                    new SimpleWorkingDaysResolver(DateTime.MinValue, DateTime.MaxValue, holidays)
                ), name);

                if (typeOfFile == "pdf")
                {
                    var pdfConverter = new PdfConverter();
                    var pdfDocument = pdfConverter.Convert(htmlDocument);
                    return this.File(pdfDocument.Content, pdfDocument.ContentType, $"document-{month.ToString("MM-yyyy")}-{name}.pdf");
                }
                else
                {
                    return this.File(htmlDocument.Content, htmlDocument.ContentType, $"document-{month.ToString("MM-yyyy")}-{name}.html");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
