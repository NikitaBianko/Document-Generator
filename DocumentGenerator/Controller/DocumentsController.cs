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
        [HttpGet]
        public IActionResult Get(string name, string workingMonth, string totalHours, string minWorkingHours, string maxWorkingHours, string endWorkingDay, string startWorkingDay, string typeOfFile)
        {
            try
            {
                var month = DateTime.Parse(workingMonth);

                var minHours = TimeSpan.FromHours(double.Parse(minWorkingHours));

                var maxHours = TimeSpan.FromHours(double.Parse(maxWorkingHours));

                var startWorking = TimeSpan.Parse(startWorkingDay);

                var endWorking = TimeSpan.Parse(endWorkingDay);

                var hours = TimeSpan.FromHours(double.Parse(totalHours));

                if(name == null)
                {
                    ModelState.AddModelError("name", "name is empty");
                }

                if(startWorking > endWorking)
                {
                    ModelState.AddModelError("endWorking", "Work end time is less than start time");
                }

                if(minHours > maxHours)
                {
                    ModelState.AddModelError("maxWorkingHours", "Maximum running time is less than minimum");
                }

                if(typeOfFile != "pdf" && typeOfFile != "html")
                {
                    ModelState.AddModelError("typeOfFile", "Document type not defined");
                }

                if(startWorking + maxHours > endWorking)
                {
                    ModelState.AddModelError("maxHours", "the period from the beginning of the working day to the end of the working day is less than the maximum working time");
                }

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
