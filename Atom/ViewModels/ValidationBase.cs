using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Atom.Annotations;
using Newtonsoft.Json;

namespace Atom.ViewModels
{
    public class ValidationBase : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private readonly object _lock = new object();
        [JsonIgnore]
        public bool HasErrors { get { return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0); } }
        [JsonIgnore]
        public bool IsValid { get { return !this.HasErrors; } }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        public bool IsPropertyValid(string propertyName)
        {
            return !_errors.ContainsKey(propertyName);
        }
        public void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

            OnPropertyChanged(nameof(IsValid));
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName))
                    _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        protected void SetError(string propertyName, string error)
        {
            
        }
        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = _errors.Keys.ToList();
                _errors.Clear();
                propNames.ForEach(pn => OnErrorsChanged(pn));
                HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            //Group validation results by property names  
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;
            //add _errors to dictionary and inform binding engine about _errors  
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }  
}
