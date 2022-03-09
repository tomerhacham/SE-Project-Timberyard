using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.API.Controllers.ModelValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringIsNotNullOrEmptyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string str = (string)value;
            if (string.IsNullOrEmpty(str))
            {
                return new ValidationResult($"This field cannot be Null or Empty");
            }
            else { return ValidationResult.Success; }
        }
    }
}
