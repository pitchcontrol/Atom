using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly string _errorMessage;

        public MaxValueAttribute(string propertyName, string errorMessage = null)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo pi = validationContext.ObjectType.GetProperty(_propertyName);
            int maxValue = (int)pi.GetValue(validationContext.ObjectInstance);
            int currentValue = (int)value;
            if (currentValue > maxValue - 1)
            {
                return new ValidationResult(_errorMessage ?? "Не может быть больше", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
