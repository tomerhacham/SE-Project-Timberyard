using System;
using System.ComponentModel.DataAnnotations;

namespace WebService.API.Controllers.ModelValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringIsNotNullOrEmptyAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate the the provided value is non empty string nor null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
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
