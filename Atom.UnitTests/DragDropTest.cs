using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class DragDropTest
    {
        [Test(Description = "Из панели перетаскиваем в корень")]
        public void DropTest1()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            RootPanel rootPanel = new RootPanel(properties);
            Panel panel = new Panel(rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент" };
            rootPanel.Children.Add(panel);
            ModalViewModel control = new ModalViewModel(panel.Children)
            {
                Type = "varchar",
                FieldInDb = "field2",
                RuDescription = "Комент",
                ControlIdView = "lb2"
            };
            panel.Children.Add(control);

            Assert.AreEqual(1, panel.Children.Count);
            Assert.AreEqual(1, rootPanel.Children.Count);
            //Перебрасываем контрол
            rootPanel.Drop(control);
            //В панели больше нет детей
            Assert.AreEqual(0,panel.Children.Count);
            Assert.AreEqual(2, rootPanel.Children.Count);
            Assert.AreEqual(control.ParentCollection, rootPanel.Children);
        }
        [Test(Description = "Из корня в дочернею панель")]
        public void DropTest2()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            RootPanel rootPanel = new RootPanel(properties);
            ModalViewModel control = new ModalViewModel(rootPanel.Children)
            {
                Type = "varchar",
                FieldInDb = "field2",
                RuDescription = "Комент",
                ControlIdView = "lb2"
            };
            rootPanel.Children.Add(control);
            Panel panel = new Panel(rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент" };
            rootPanel.Children.Add(panel);
            Assert.AreEqual(2, rootPanel.Children.Count);
            Assert.AreEqual(0, panel.Children.Count);
            //Перебрасываем контрол
            panel.Drop(control);
            Assert.AreEqual(1, panel.Children.Count);
            Assert.AreEqual(1, rootPanel.Children.Count);
            Assert.AreEqual(control.ParentCollection, panel.Children);
        }
    }
}
