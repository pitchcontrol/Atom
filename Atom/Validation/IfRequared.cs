using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Atom.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IfRequired : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly string _errorMessage;

        public IfRequired(string propertyName, string errorMessage = null)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo pi = validationContext.ObjectType.GetProperty(_propertyName);
            bool active = (bool)pi.GetValue(validationContext.ObjectInstance);

            if (!active)
                return ValidationResult.Success;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(_errorMessage ?? "Не может быть пустым", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
