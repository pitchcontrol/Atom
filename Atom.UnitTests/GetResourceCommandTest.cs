﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Commands;
using Atom.Models;
using Atom.Services;
using Atom.ViewModels;
using Caliburn.Micro;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class GetResourceCommandTest
    {
        RootPanel _rootPanel;
        private GetResourceCommand _command;
        private ConstructorViewModel _model;
        [SetUp]
        public void Init()
        {
            //ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            //_rootPanel = new RootPanel(properties);
            //properties.Add(_rootPanel);
            _model = new ConstructorViewModel(new EventAggregator(), new WindowManager());
            _rootPanel = _model.RootPanel;
            //properties.Clear();

            //foreach (WebPageBaseViewModel webPageBaseViewModel in _model.Properties)
            //{
            //    _model.Properties.Add(webPageBaseViewModel);
            //}

            _command = new GetResourceCommand(_model);

        }
        [Test]
        public void SimpleTest()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", EnDescription = "Comment", ControlIdView = "lb1" });
            PanelViewModel panelViewModel = new PanelViewModel(_rootPanel) { ControlIdView = "clId1", RuDescription = "Комент", EnDescription = "Comment"};
            _rootPanel.Children.Add(panelViewModel);
            //Вторая панель вложена в первую
            PanelViewModel panel2 = new PanelViewModel(panelViewModel) { ControlIdView = "clId2", RuDescription = "Комент", EnDescription = "Comment" };
            panelViewModel.Children.Add(panel2);
            panel2.Children.Add(new ModalViewModel(_rootPanel) { Type = "varchar", FieldInDb = "field2", RuDescription = "Комент", EnDescription = "Comment", ControlIdView = "lb2" });
            _command.Execute("False");

            string result = "<data name=\"lb1\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"clId1\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"clId2\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"lb2\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";

            //Assert.AreEqual(result,"");
        }
    }
}
