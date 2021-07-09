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
        
        public Document Generate(WorkingHoursParams @params)
        {
            var workingHours = new WorkingHoursCalculator(@params).Calculate();
            var document = this.documentTemplate.Generate(new
            {
                WorkingHours = workingHours.Select(x => new
                {
                    date = x.Day.ToString("dd.MM.yyyy"),
                    begin = x.BeginningOfWork.ToString("hh\\:mm"),
                    ende = x.EndOfWork.ToString("hh\\:mm"),
                    durationWork = x.DurationOfWork.ToString("hh\\:mm")
                }).ToList()
            });

            return document;
        }
    }
}