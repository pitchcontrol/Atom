using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Validation
{
    /// <summary>
    /// Свойство становится обязательным если другое свойство равно определенному значению
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TriggerRequired : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly string _errorMessage;
        private readonly IEnumerable<string> _propertyValues;

        public TriggerRequired(string propertyName, string[] propertyValues,string errorMessage = null)
        {
            _propertyName = propertyName;
            _propertyValues = propertyValues;
            _errorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo pi = validationContext.ObjectType.GetProperty(_propertyName);
            string currentValue = (string)pi.GetValue(validationContext.ObjectInstance);

            if (!_propertyValues.Contains(currentValue))
                return ValidationResult.Success;
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(_errorMessage ?? "Не может быть пустым", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
