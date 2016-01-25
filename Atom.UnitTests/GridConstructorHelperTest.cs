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
    public class GridConstructorHelperTest
    {
        RootPanel _rootPanel;
        private GridConstructorHelper _helper;
        [SetUp]
        public void Init()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(properties);
            properties.Add(_rootPanel);
            _helper = new GridConstructorHelper();
        }
        [Test]
        public void SimpleTest()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel) { Type = "bit", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2" });
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel) { Type = "file", FieldInDb = "field3", RuDescription = "Комент", ControlIdView = "lb3" });
            
            _helper.ResourceNamespace = "RefBook";
            _helper.Construct(_rootPanel.Children);

            string result = "<gp:GpGridView ID=\"gvObject\" Height=\"100%\" runat=\"server\" PageSize=\"10\" ShowWhenEmpty=\"False\" SkinID=\"gpGridView\" DataKeyNames=\"id\" PagerSettings-Visible=\"true\">\n";
            result += "<Columns>\n";
            result += "<%--Комент--%>\n";
            result += "<asp:TemplateField HeaderText=\"<%$ Resources: RefBook, lb1 %>\" SortExpression=\"field1\" AccessibleHeaderText=\"field1\">\n";
            result +="<ItemTemplate>\n";
            result += "<asp:Label ID=\"lb1\" runat=\"server\"><%# Eval(\"field1\")%></asp:Label>\n";
            result +="</ItemTemplate>\n";
            result += "</asp:TemplateField>\n";
            result += "<%--Комент--%>\n";
            result += "<asp:TemplateField HeaderText=\"<%$ Resources: RefBook, lb2 %>\" SortExpression=\"field2\" AccessibleHeaderText=\"field2\">\n";
            result += "<ItemTemplate>\n";
            result += "<asp:CheckBox  ID=\"lb2\" runat=\"server\" value=\"<%# Eval(\"field2\")%>\"/>\n";
            result += "</ItemTemplate>\n";
            result += "</asp:TemplateField>\n";
            result += "<%--Комент--%>\n";
            result += "<asp:TemplateField HeaderText=\"<%$ Resources: RefBook, lb3 %>\" SortExpression=\"field3\" AccessibleHeaderText=\"field3\">\n";
            result += "<ItemTemplate>\n";
            result += "<gp:ValidatingFileView ID=\"lb3\" runat=\"server\" SkinID=\"ViewModeSkin\" FileName=\"<%# Eval(\"fileName\")%>\" Value='<%# (Eval(\"field3\") is DBNull) ? -1 : Eval(\"field3\") %>'/>\n";
            result += "</ItemTemplate>\n";
            result += "</asp:TemplateField>\n";
            result += "</Columns>\n";
            result += "</gp:GpGridView>\n";
            Assert.AreEqual(result, _helper.ToString());
        }
    }
}
