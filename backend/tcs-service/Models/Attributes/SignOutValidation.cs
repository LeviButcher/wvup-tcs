using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SignOutValidation : ValidationAttribute
    {
        public SignOutValidation(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            DateTime outTime = (DateTime)value;

            DateTime inTime = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (outTime > inTime)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Date is not later");
            }
        }
    }

}
