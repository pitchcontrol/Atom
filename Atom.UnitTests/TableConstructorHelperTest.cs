using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Services;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class TableConstructorHelperTest
    {
        RootPanel _rootPanel;
        private TableConstructorHelper _helper;
        private ObservableCollection<WebPageBaseViewModel> _properties;
        [SetUp]
        public void Init()
        {
            _properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(_properties);
            _properties.Add(_rootPanel);
            _helper = new TableConstructorHelper();
        }
        [TearDown]
        public void Cleanup()
        {
            File.Delete("../../Table.sql");
            File.Delete("../../Table_id.sql");
        }
        [Test(Description = "Простой случай")]
        public void OneTableTest()
        {
            ModalViewModel model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field1";
            model.Type = "int";
            model.TableName = "Table";
            _rootPanel.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field2";
            model.Type = "varchar";
            model.TableName = "Table";
            _rootPanel.Children.Add(model);

            _helper.Construct(_properties, "../..");

            string tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../Table.sql"));
            string expectedContent = Utils.ReplaceSpaces("CREATE TABLE [dbo].[Table] (\n" +
                                                         "[pkid] INT IDENTITY (1, 1)  NOT NULL,\n" +
                                                         "[fl_del] INT CONSTRAINT [DF_Table_del] DEFAULT ((0)) NULL,\n" +
                                                         "[idreq] INT NULL,\n" +
                                                         "[dats] datetime NOT NULL,\n" +
                                                         "[datf] datetime NOT NULL,\n" +
                                                         "[idcp] INT NULL,\n" +
                                                         "[fldchange] varchar(MAX) NULL,\n" +
                                                         "[idRecord] INT NOT NULL,\n" +
                                                         "[idul] INT NOT NULL,\n" +
                                                         "[Field1] INT,\n" +
                                                         "[Field2] varchar(max),\n" +
                                                         "CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED([pkid] ASC),\n" +
                                                         "CONSTRAINT [FK_Table_id] FOREIGN KEY (idRecord) REFERENCES [Table_id]([pkid]),\n" +
                                                         "CONSTRAINT [FK_Table_ul] FOREIGN KEY (idul) REFERENCES [ul]([pkid])\n" +
                                                         ")");
            Assert.AreEqual(expectedContent, tableContent);

            //Ид таблица
            tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../Table_id.sql"));
            expectedContent = Utils.ReplaceSpaces("CREATE TABLE [dbo].[Table_id] (\n" +
                "[pkid] INT IDENTITY(1, 1) NOT NULL,\n" +
                "[fl_del] INT CONSTRAINT[DF_Table_id_del] DEFAULT((0)) NULL,\n" +
                "CONSTRAINT[PK_Table_id] PRIMARY KEY CLUSTERED([pkid] ASC)\n" +
                ")\n");
            Assert.AreEqual(expectedContent, tableContent);
        }

    }
}
