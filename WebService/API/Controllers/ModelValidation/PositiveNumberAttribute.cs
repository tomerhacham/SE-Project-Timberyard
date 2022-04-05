using System;
using System.ComponentModel.DataAnnotations;

namespace WebService.API.Controllers.ModelValidation
{
    /// <summary>
    /// Attribute to enforce only positive values (exclude 0)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PositiveNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((int)value > 0)
            {
                return new ValidationResult($"This field cannot be non positive value");
            }
            else { return ValidationResult.Success; }
        }
    }
}
