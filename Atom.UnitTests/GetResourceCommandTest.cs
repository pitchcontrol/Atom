using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Commands;
using Atom.Services;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class GetResourceCommandTest
    {
        RootPanel _rootPanel;
        private GetResourceCommand _command;
        private MainViewModel _model;
        [SetUp]
        public void Init()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(properties);
            properties.Add(_rootPanel);
            _model = new MainViewModel();
            _model.Properties.Clear();
            foreach (WebPageBaseViewModel webPageBaseViewModel in properties)
            {
                _model.Properties.Add(webPageBaseViewModel);
            }

            _command = new GetResourceCommand(_model);

        }
        [Test]
        public void SimpleTest()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", EnDescription = "Comment", ControlIdView = "lb1" });
            Panel panel = new Panel(_rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент", EnDescription = "Comment"};
            _rootPanel.Children.Add(panel);
            //Вторая панель вложена в первую
            Panel panel2 = new Panel(panel.Children) { ControlIdView = "clId2", RuDescription = "Комент", EnDescription = "Comment" };
            panel.Children.Add(panel2);
            panel2.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "varchar", FieldInDb = "field2", RuDescription = "Комент", EnDescription = "Comment", ControlIdView = "lb2" });
            _command.Execute("False");

            string result = "<data name=\"lb1\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"clId1\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"clId2\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";
            result += "<data name=\"lb2\" xml:space=\"preserve\">\n";
            result += "<value>Комент</value>\n</data>\n";

            Assert.AreEqual(result,_model.ResuorseTextRu);
        }
    }
}
