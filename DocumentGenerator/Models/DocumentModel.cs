using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentGenerator.Models
{
    public class DocumentModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public DateTime workingMonth { get; set; } = DateTime.Now;
        [Required]
        public double totalHours { get; set; }
        [Required]
        public double minWorkingHours { get; set; }
        [Required]
        //[MaxWorkingHours]
        public double maxWorkingHours { get; set; }
        [Required]
        public string startWorkingDay { get; set; }
        [Required]
        //[EndWorkingDay]
        public string endWorkingDay { get; set; }
        [Required]
        public string typeOfFile { get; set; } = "pdf";
    }
}
