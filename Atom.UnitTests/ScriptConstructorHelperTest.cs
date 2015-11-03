using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Services;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class ScriptConstructorHelperTest
    {
        RootPanel _rootPanel;
        private ScriptConstructorHelper _helper;
        [SetUp]
        public void Init()
        {
            ObservableCollection<WebPageBaseViewModel> properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(properties);
            properties.Add(_rootPanel);
            _helper = new ScriptConstructorHelper();
        }
        [Test]
        public void SimpleViewTest()
        {
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "int", FieldInDb = "field1", RuDescription = "Комент", ControlIdView = "lb1" });
            Panel panel = new Panel(_rootPanel.Children) { RuDescription = "Комент", FieldInDb = "Id1" };
            _rootPanel.Children.Add(panel);
            panel.Children.Add(new ModalViewModel(_rootPanel.Children) { Type = "varchar", FieldInDb = "field2", RuDescription = "Комент", ControlIdView = "lb2" });

            string result = "DECLARE @id int;\n";
            //field1
            result += "--field1\n";
            result +=
                "INSERT INTO [ut_MenuField] (idpage,fld, idparent, fldbd, tabbd, isNotEdited, nam) VALUES (100, 'lb1', null, 'field1', null, 0, 'ru-RU:Комент;en-EN:;');\n";
            result += "set @id  = scope_identity();\n";
            result += "insert into [ut_RoleField] (idrole, idfld,visability)\nvalues\n";
            result += "(3,@id,1)\n";
            //clId1
            result += "--Id1\n";
            result +=
                "INSERT INTO [ut_MenuField] (idpage,fld, idparent, fldbd, tabbd, isNotEdited, nam) VALUES (100, 'cpId1', null, 'Id1', null, 0, 'ru-RU:Комент;en-EN:;');\n";
            result += "set @id  = scope_identity();\n";
            result += "insert into [ut_RoleField] (idrole, idfld,visability)\nvalues\n";
            result += "(3,@id,1)\n";
            //field2
            result += "--field2\n";
            result +=
                "INSERT INTO [ut_MenuField] (idpage,fld, idparent, fldbd, tabbd, isNotEdited, nam) VALUES (100, 'lb2', null, 'field2', null, 0, 'ru-RU:Комент;en-EN:;');\n";
            result += "set @id  = scope_identity();\n";
            result += "insert into [ut_RoleField] (idrole, idfld,visability)\nvalues\n";
            result += "(3,@id,1)\n";
            _helper.Constructor(_rootPanel.Children, false, 100, 3);
            Assert.AreEqual(result, _helper.ToString());
        }
    }
}
