using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebService.API.Controllers.ModelValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidEmailsAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate the the provided value is list of valid emails
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> emails = (List<string>)value;
            foreach (var email in emails)
            {
                if (!ValidEmailAttribute.IsValidEmail(email))
                {
                    return new ValidationResult($"This email address {email} is not valid");
                }
            }
            return ValidationResult.Success;
        }

    }
}
