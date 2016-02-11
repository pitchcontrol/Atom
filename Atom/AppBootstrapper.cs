using Atom.Interfaces;
using Atom.Models;
using Atom.Services;
using Atom.ViewModels;
using Microsoft.Practices.Unity;

namespace Atom
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class AppBootstrapper : BootstrapperBase
    {
        readonly IUnityContainer _container = new UnityContainer();

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.RegisterType<IWindowManager, WindowManager>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ShellViewModel>(new ContainerControlledLifetimeManager());
        }

        protected override object GetInstance(Type service, string key)
        {
            if (key == "ShellViewModel")
                return _container.Resolve<ShellViewModel>();

            var instance = _container.Resolve(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}