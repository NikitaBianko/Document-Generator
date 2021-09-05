using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class EndWorkingDayAttribute : ValidationAttribute
    { 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return new ValidationResult("The UserName cannot contain the word admin",
                new[] { validationContext.MemberName });
        }

    }
}