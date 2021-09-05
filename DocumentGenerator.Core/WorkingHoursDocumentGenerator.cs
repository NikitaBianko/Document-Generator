using System;
using System.Linq;

namespace DocumentGenerator.Core
{
    public class WorkingHoursDocumentGenerator
    {
        private readonly IDocumentTemplate documentTemplate;

        public WorkingHoursDocumentGenerator(IDocumentTemplate documentTemplate)
        {
            this.documentTemplate = documentTemplate;
        }
        
        public Document Generate(WorkingHoursParams @params, string name)
        {
            var workingHours = new WorkingHoursCalculator(@params).Calculate();
            var document = this.documentTemplate.Generate(new
            {
                name = name,
                WorkingHours = workingHours.Select(x => 
                new
                {
                    dateType = x.Day.Year != 1 ? @params.WorkingDayResolver.IsWorkingDay(x.Day) ? "working_day" : "holiday" : "empty_day",
                    date = x.Day.Year != 1 ? $"{x.Day.ToString("dd.MM.yyyy")}({x.Day.DayOfWeek})" : "",
                    begin = x.DurationOfWork.Ticks != 0 ? x.BeginningOfWork.ToString("hh\\:mm") : "",
                    ende = x.DurationOfWork.Ticks != 0 ? x.EndOfWork.ToString("hh\\:mm") : "",
                    durationWork = x.DurationOfWork.Ticks != 0 ? x.DurationOfWork.ToString("hh\\:mm") : ""
                }).ToList()
            });

            return document;
        }
    }
}