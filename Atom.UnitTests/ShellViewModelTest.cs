using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Services;
using Atom.ViewModels;
using Caliburn.Micro;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class ShellViewModelTest
    {
        private ConstructorViewModel _model;
        [SetUp]
        public void Init()
        {
            _model = new ConstructorViewModel(new EventAggregator(),new WindowManager());
        }
        [Test]
        public void TurnGridTest()
        {
            PanelViewModel panelViewModel = new PanelViewModel(_model.RootPanel) { RuDescription = "Комент", FieldInDb = "Id1" };
            _model.RootPanel.Children.Add(panelViewModel);
            ModalViewModel panel1 = new ModalViewModel(panelViewModel)
            {
                Type = "varchar",
                FieldInDb = "field2",
                RuDescription = "Комент",
                ControlIdView = "lb2"
            };
            panelViewModel.Children.Add(panel1);
            ModalViewModel panel2 = new ModalViewModel(panelViewModel)
            {
                Type = "varchar",
                FieldInDb = "field3",
                RuDescription = "Комент",
                ControlIdView = "lb3"
            };
            panelViewModel.Children.Add(panel2);

            _model.CurrentProperty = panelViewModel;

            int index = _model.RootPanel.Children.IndexOf(panelViewModel);
            int childrenCount = panelViewModel.Children.Count;

            _model.TurnGrid();
            
            //Проверяем тип
            WebPageBaseViewModel newModel = _model.RootPanel.Children[index];
            Assert.IsTrue(newModel is GridViewModel);
            //Количество детей
            Assert.AreEqual(childrenCount, newModel.Children.Count);
            //Проверяем родительские колекции
            Assert.AreEqual(childrenCount, panel1.ParentCollection.Count);
            Assert.AreEqual(childrenCount, panel2.ParentCollection.Count);
        }
    }
}
