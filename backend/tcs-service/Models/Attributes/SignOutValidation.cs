using System;
using System.ComponentModel.DataAnnotations;

namespace tcs_service.Models.Attributes
{
    ///<summary>Ensures that a Sign In inTime and outTime is valid</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SignOutValidation : ValidationAttribute
    {
        ///<summary>SignOutValidation Constructor</summary>
        public SignOutValidation(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        ///<summary>Ensures that outTime is after the inTime property</summary>
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
