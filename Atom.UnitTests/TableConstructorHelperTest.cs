using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Constant;
using Atom.Services;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class TableConstructorHelperTest
    {
        private RootPanel _rootPanel;
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
            File.Delete("../../GridTable.sql");
            File.Delete("../../GridTable_id.sql");
        }

        [Test(Description = "Простой случай")]
        public void OneTableTest()
        {
            _rootPanel.TableName = "Table";
            ModalViewModel model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field1";
            model.Type = "int";
            _rootPanel.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field2";
            model.Type = "varchar";
            _rootPanel.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field3";
            model.Type = ControlTypes.Dictionary;
            model.DictionaryType = DictionaryTypes.FlName;
            _rootPanel.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field4";
            model.Type = ControlTypes.Dictionary;
            model.DictionaryType = DictionaryTypes.SimpleDictionary;
            model.DictionaryTableName = "MyDictionary";
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
                                                         "[Field3] INT,\n" +
                                                         "[Field4] INT,\n" +
                                                         "CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED([pkid] ASC),\n" +
                                                         "CONSTRAINT [FK_Table_id] FOREIGN KEY (idRecord) REFERENCES [Table_id]([pkid]),\n" +
                                                         "CONSTRAINT [FK_Table_ul] FOREIGN KEY (idul) REFERENCES [ul]([pkid]),\n" +
                                                         "CONSTRAINT [FK_Table_Field3_fl] FOREIGN KEY (Field3) REFERENCES [fl]([pkid]),\n" +
                                                         "CONSTRAINT [FK_Table_Field4_MyDictionary] FOREIGN KEY (Field4) REFERENCES [MyDictionary]([pkid])\n" +
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

        [Test(Description = "Простой случай грид")]
        public void OneGridTest()
        {
            GridViewModel grid = new GridViewModel(_rootPanel);
            grid.TableName = "GridTable";
            _rootPanel.Children.Add(grid);

            ModalViewModel model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field1";
            model.Type = "int";
            grid.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field2";
            model.Type = "varchar";
            grid.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field3";
            model.Type = ControlTypes.Dictionary;
            model.DictionaryType = DictionaryTypes.FlName;
            grid.Children.Add(model);

            _helper.Construct(_properties, "../..");

            string tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../GridTable.sql"));
            string expectedContent = Utils.ReplaceSpaces("CREATE TABLE [dbo].[GridTable] (\n" +
                                                         "[pkid] INT IDENTITY (1, 1)  NOT NULL,\n" +
                                                         "[fl_del] INT CONSTRAINT [DF_GridTable_del] DEFAULT ((0)) NULL,\n" +
                                                         "[idreq] INT NULL,\n" +
                                                         "[dats] datetime NOT NULL,\n" +
                                                         "[datf] datetime NOT NULL,\n" +
                                                         "[idcp] INT NULL,\n" +
                                                         "[fldchange] varchar(MAX) NULL,\n" +
                                                         "[idRecord] INT NOT NULL,\n" +
                                                         "[externalId] INT NOT NULL,\n" +
                                                         "[Field1] INT,\n" +
                                                         "[Field2] varchar(max),\n" +
                                                         "[Field3] INT,\n" +
                                                         "CONSTRAINT [PK_GridTable] PRIMARY KEY CLUSTERED([pkid] ASC),\n" +
                                                         "CONSTRAINT [FK_GridTable_id] FOREIGN KEY (idRecord) REFERENCES [GridTable_id]([pkid]),\n" +
                                                         "CONSTRAINT [FK_GridTable_Field3_fl] FOREIGN KEY (Field3) REFERENCES [fl]([pkid])\n" +
                                                         ")");
            Assert.AreEqual(expectedContent, tableContent);

            //Ид таблица
            tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../GridTable_id.sql"));
            expectedContent = Utils.ReplaceSpaces("CREATE TABLE [dbo].[GridTable_id] (\n" +
                                                  "[pkid] INT IDENTITY(1, 1) NOT NULL,\n" +
                                                  "[fl_del] INT CONSTRAINT[DF_GridTable_id_del] DEFAULT((0)) NULL,\n" +
                                                  "CONSTRAINT[PK_GridTable_id] PRIMARY KEY CLUSTERED([pkid] ASC)\n" +
                                                  ")\n");
            Assert.AreEqual(expectedContent, tableContent);

        }
    }
}
