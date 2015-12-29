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
    public class PageConstructotHelperTest
    {
        RootPanel _rootPanel;
        private PageConstructotHelper _helper;
        [SetUp]
        public void Init()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(properties);
            properties.Add(_rootPanel);
            _helper = new PageConstructotHelper();
        }

        [TestCase("int", "field1", "lb1", "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb1\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("varchar", "field2", "lb2", "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb2\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb2 %>\" DataBoundField=\"field2\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("datetime", "field3", "lb3", "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb3\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb3 %>\" DataBoundField=\"field3\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("decimal", "field4", "lb4", "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb4\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb4 %>\" DataBoundField=\"field4\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("dictionary", "field5", "lb5", "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb5\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb5 %>\" DataBoundField=\"field5\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("bit", "field6", "bllb6", "<%--Комент--%>\n<gp:ValidatingBoolLabel ID=\"bllb6\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , bllb6 %>\" DataBoundField=\"field6\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("file", "field7", "fllb7", "<%--Комент--%>\n<gp:ValidatingFileView ID=\"fllb7\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , fllb7 %>\" DataBoundField=\"field7\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        //Тест простой вьюхи
        public void ViewSimpleTest(string type, string fieldName, string id, string result)
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = type, FieldInDb = fieldName, RuDescription = "Комент", ControlIdView = id });
            _helper.Construct(_rootPanel.Children, false);
            Assert.AreEqual(result, _helper.ToString());
        }
        [Test(Description = "Отображаем пустую панель")]
        public void SimplePanel()
        {
            _rootPanel.Children.Add(new Panel(_rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент" });
            _helper.Construct(_rootPanel.Children, false);
            string result = "<%--Комент--%>\n<gp:CollapsePanel ID=\"clId1\" runat=\"server\" Caption=\"<%$ Resources: , clId1 %>\" SkinID=\"CollapsePanel\">\n</gp:CollapsePanel>\n";
            Assert.AreEqual(result, _helper.ToString());
        }

        [TestCase("int", "field1", "lb1", "<%--Комент--%>\n<gp:ValidatingTextBox ID=\"lb1\" runat=\"server\" sqlType=\"Int\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtxtfield1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("varchar", "field2", "lb2", "<%--Комент--%>\n<gp:ValidatingTextBox ID=\"lb2\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtxtfield2 %>\" DataBoundField=\"field2\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("datetime", "field3", "lb3", "<%--Комент--%>\n<gp:ValidatingJsCalendar ID=\"lb3\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtclfield3 %>\" DataBoundField=\"field3\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\" ImageUrl=\"~/Images/week_small.gif\"  ValidType=\"FORM_ERROR_TYPE_DATE\" />\n")]
        [TestCase("decimal", "field4", "lb4", "<%--Комент--%>\n<gp:ValidatingTextBox ID=\"lb4\" runat=\"server\" sqlType=\"Decimal\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtxtfield4 %>\" DataBoundField=\"field4\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("dictionary", "field5", "lb5", "<%--Комент--%>\n<gp:ValidatingDropDawnList ID=\"lb5\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vddlfield5 %>\" DataBoundField=\"field5\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("bit", "field6", "bllb6", "<%--Комент--%>\n<gp:ValidatingBoolLabel ID=\"bllb6\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , field6 %>\" DataBoundField=\"field6\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        [TestCase("file", "field7", "fllb7", "<%--Комент--%>\n<gp:ValidatingFileView ID=\"fllb7\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtclfield7 %>\" DataBoundField=\"field7\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n")]
        public void EditSimplyTest(string type, string fieldName, string id, string result)
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = type, FieldInDb = fieldName, RuDescription = "Комент", ControlIdView = id });
            _helper.Construct(_rootPanel.Children, true);
            Assert.AreEqual(result, _helper.ToString());
        }
        [Test(Description = "Контрол, панель и контрол внутри")]
        public void ComplexTest1()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            Panel panel = new Panel(_rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент" };
            _rootPanel.Children.Add(panel);
            panel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "varchar", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2" });
            string result = "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb1\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            result +=
                "<%--Комент--%>\n<gp:CollapsePanel ID=\"clId1\" runat=\"server\" Caption=\"<%$ Resources: , clId1 %>\" SkinID=\"CollapsePanel\">\n";
            result +=
                "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb2\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb2 %>\" DataBoundField=\"field2\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            result += "</gp:CollapsePanel>\n";
            _helper.Construct(_rootPanel.Children, false);
            Assert.AreEqual(result, _helper.ToString());
        }

        [Test(Description = "Контрол, панель, внутри панель и контрол внутри")]
        public void ComplexTest2()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            Panel panel = new Panel(_rootPanel.Children) { ControlIdView = "clId1", RuDescription = "Комент" };
            _rootPanel.Children.Add(panel);
            //Вторая панель вложена в первую
            Panel panel2 = new Panel(panel.Children) { ControlIdView = "clId2", RuDescription = "Комент" };
            panel.Children.Add(panel2);
            panel2.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "varchar", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2" });

            string result = "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb1\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            result +=
                "<%--Комент--%>\n<gp:CollapsePanel ID=\"clId1\" runat=\"server\" Caption=\"<%$ Resources: , clId1 %>\" SkinID=\"CollapsePanel\">\n";
            result += "<%--Комент--%>\n<gp:CollapsePanel ID=\"clId2\" runat=\"server\" Caption=\"<%$ Resources: , clId2 %>\" SkinID=\"CollapsePanel\">\n";
            result +=
                "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb2\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , lb2 %>\" DataBoundField=\"field2\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            result += "</gp:CollapsePanel>\n";
            result += "</gp:CollapsePanel>\n";
            _helper.Construct(_rootPanel.Children, false);
            Assert.AreEqual(result, _helper.ToString());
        }
        [Test(Description = "Проверяем добавление только редактируемых")]
        public void IsEditableTest()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2", IsEditable = false });
            _helper.Construct(_rootPanel.Children, true);
            string result = "<%--Комент--%>\n<gp:ValidatingTextBox ID=\"lb1\" runat=\"server\" sqlType=\"Int\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: , vtxtfield1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            Assert.AreEqual(result, _helper.ToString());
        }
        [Test]
        public void NamespaceTest()
        {
            _helper.ResourceNamespace = "RefBook";
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2" });
            _helper.Construct(_rootPanel.Children, false);
            string result = "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb1\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: RefBook, lb1 %>\" DataBoundField=\"field1\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            result += "<%--Комент--%>\n<gp:ValidatingLabel ID=\"lb2\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"<%$ Resources: RefBook, lb2 %>\" DataBoundField=\"field2\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n";
            Assert.AreEqual(result, _helper.ToString());
        }
    }
}
