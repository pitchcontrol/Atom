using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.ViewModels
{
    public class EnterNameViewModel : ValidationBase
    {
        private string _description;
        private string _value;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        [Required]
        public string Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                ValidateProperty(nameof(Value));
                OnPropertyChanged();
            }
        }
    }
}
