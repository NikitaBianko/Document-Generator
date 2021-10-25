using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    internal class DivisibilityTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value.GetType().Equals(typeof(double)))
                return (double)value % 0.5 == 0;

            var hour = TimeSpan.Parse((string)value);

            return hour.TotalHours % 0.5 == 0;
        }
    }
}