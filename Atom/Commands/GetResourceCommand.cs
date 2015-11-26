using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Atom.Commands
{
    public class GetResourceCommand:ICommand
    {
        private readonly MainViewModel _model;

        public GetResourceCommand(MainViewModel model)
        {
            _model = model;
            _model.Properties.CollectionChanged += (s, e) =>
            {
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
            };
        }

        public bool CanExecute(object parameter)
        {
            return _model.Properties.Count != 0;
        }

        public void Execute(object parameter)
        {
            string result = "";
            if (parameter.ToString() == "ru")
            {
                foreach (WebPageBaseViewModel modalViewModel in _model.Properties)
                {
                    result += string.Format("<data name=\"{0}\" xml:space=\"preserve\">\n", modalViewModel.FieldInDb);
                    result += string.Format("<value>{0}</value>\n</data>\n", modalViewModel.RuDescription);
                }
            }
            else
            {
                foreach (WebPageBaseViewModel modalViewModel in _model.Properties)
                {
                    result += string.Format("<data name=\"{0}\" xml:space=\"preserve\">\n", modalViewModel.FieldInDb);
                    result += string.Format("<value>{0}</value>\n</data>\n", modalViewModel.RuDescription);
                }
            }
            _model.ResuorseText = result;
        }

        public event EventHandler CanExecuteChanged;
    }
}
