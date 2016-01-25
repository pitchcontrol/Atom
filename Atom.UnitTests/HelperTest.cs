using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Services;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class HelperTest
    {
        private RootPanel _rootPanel;
        [SetUp]
        public void Init()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(properties);
            properties.Add(_rootPanel);
        }
        [Test]
        public void FlattenTest()
        {
            PanelViewModel panel1 = new PanelViewModel(_rootPanel);
            _rootPanel.Children.Add(panel1);
            PanelViewModel panel2 = new PanelViewModel(panel1);
            panel1.Children.Add(panel2);
            PanelViewModel panel3 = new PanelViewModel(panel2);
            panel2.Children.Add(panel3);
            var count =_rootPanel.Children.Flatten(i => i.Children).Count();
            Assert.AreEqual(3,count);
        }
    }
}
