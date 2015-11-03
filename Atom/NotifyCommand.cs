using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Atom
{
    public class NotifyCommand<T> : ICommand where T : INotifyPropertyChanged
    {
        private readonly T _model;
        private readonly IEnumerable<string> _properties;
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public NotifyCommand(T model, IEnumerable<string> properties,  Action<T> execute, Predicate<T> canExecute)
        {
            _model = model;
            _properties = properties;
            _execute = execute;
            _canExecute = canExecute;
            _model.PropertyChanged += (s, e) =>
            {
                if(_properties.Contains(e.PropertyName))
                {
                    if(_canExecute(_model))
                        RaiseCanExecuteChanged();
                }
            };

            
            
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(_model);
        }

        public void Execute(object parameter)
        {
            _execute(_model);
        }

        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}