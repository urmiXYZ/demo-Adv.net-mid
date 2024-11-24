using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace demo.Models
{
    public class EstablishedYearValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int currentYear = DateTime.Now.Year;
                if (year < 1900 || year > currentYear)
                {
                    return new ValidationResult($"Established year must be between 1900 and {currentYear}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}