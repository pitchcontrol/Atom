using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Atom.ViewModels
{
    public class EnterNameViewModel : ValidationScreen
    {
        private string _description;
        private string _value;

        public static EnterNameViewModel CreateEnterNamespace()
        {
            var model = new EnterNameViewModel
            {
                Description = "Пространство имен",
                DisplayName = "Введите пространство имен"
            };
            return model;
        }

        public EnterNameViewModel()
        {
            Description = "Имя";
            DisplayName = "Введите имя";
            Validate();
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
        }
    }
}
