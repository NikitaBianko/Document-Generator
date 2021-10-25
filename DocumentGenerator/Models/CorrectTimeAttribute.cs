using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerator.Models
{
    public class CorrectTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                TimeSpan time;

                if (value.GetType().Equals(typeof(double)))
                    time = TimeSpan.FromHours((double)value);
                else
                    time = TimeSpan.Parse((string)value);

                if (time.TotalHours > 0 && time.TotalHours < 24)
                    return true;
            }
            catch 
            {
                return false;
            }

            return false;
        }
    }
}