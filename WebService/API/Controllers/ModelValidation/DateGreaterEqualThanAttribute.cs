using System;
using System.ComponentModel.DataAnnotations;

namespace WebService.API.Controllers.ModelValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterEqualThanAttribute : ValidationAttribute
    {
        private string DateToCompareFieldName { get; set; }

        public DateGreaterEqualThanAttribute(string dateToCompareFieldName)
        {
            DateToCompareFieldName = dateToCompareFieldName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime laterDate = (DateTime)value;

            var earlierDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareFieldName).GetValue(validationContext.ObjectInstance, null);

            if (laterDate >= earlierDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"date is before {DateToCompareFieldName}");
            }
        }
    }
}
