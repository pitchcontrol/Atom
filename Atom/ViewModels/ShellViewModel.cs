using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Atom.ViewModels
{
    public class ShellViewModel : Conductor<Object>, IHandle<string>
    {
        private readonly ConstructorViewModel _constructorViewModel;
        private readonly RolePageViewModel _rolePageViewModel;
        private readonly IEventAggregator _aggregator;
        private string _info;
        private object _activeView;

        public ShellViewModel(ConstructorViewModel constructorViewModel, RolePageViewModel rolePageViewModel, IEventAggregator aggregator)
        {
            _constructorViewModel = constructorViewModel;
            _rolePageViewModel = rolePageViewModel;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            Show(constructorViewModel);
            DisplayName = "Конструктор";
        }

        public string Info
        {
            get { return _info; }
            set
            {
                if (value == _info) return;
                _info = value;
                NotifyOfPropertyChange();
            }
        }

        public void Show(object parametr)
        {
            _activeView = parametr;
            ActivateItem(parametr);
        }
        public bool CanShowConstructor => _constructorViewModel != _activeView;

        public void ShowConstructor()
        {
            _activeView = _constructorViewModel;
            ActivateItem(_constructorViewModel);
        }

        public bool CanShowRoles => _rolePageViewModel != _activeView;

        public void ShowRoles()
        {
            _activeView = _rolePageViewModel;
            ActivateItem(_rolePageViewModel);
        }

        public void Handle(string message)
        {
            Info += message;
        }
    }
}
