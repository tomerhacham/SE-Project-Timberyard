using System;
using System.ComponentModel.DataAnnotations;
using WebService.Domain.DataAccess.DTO;

namespace WebService.API.Controllers.ModelValidation
{
    /// <summary>
    /// Attribute to validate that the user Role is supported by the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidRoleNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return Enum.IsDefined(typeof(Role), value) ? ValidationResult.Success : new ValidationResult($"This field cannot be non positive value");
        }
    }
}
