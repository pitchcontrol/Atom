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
    public class ProcedureConstructorHelerTest
    {
        private RootPanel _rootPanel;
        private ProcedureConstructorHeler _helper;
        private ObservableCollection<WebPageBaseViewModel> _properties;

        [SetUp]
        public void Init()
        {
            _properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(_properties);
            _rootPanel.TableName = "MainTable";
            _properties.Add(_rootPanel);
            _helper = new ProcedureConstructorHeler();
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete("../../MainTableAdd.sql");
            File.Delete("../../MainTableDelete.sql");
        }


        private void SimpleInit()
        {
            ModalViewModel model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field1";
            model.Type = "int";
            model.RuDescription = "Поле 1";
            _rootPanel.Children.Add(model);
            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field2";
            model.Type = "varchar";
            model.RuDescription = "Поле 2";
            _rootPanel.Children.Add(model);

        }

        [Test(Description = "Процедура добавления")]
        public void AddProcedureTest()
        {
            SimpleInit();
            _helper.Construct(_properties, "../..");
            string tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../MainTableAdd.sql"));
            string expectedContent = Utils.ReplaceSpaces(File.ReadAllText("../../ProcedureConstructorHelerTest/AddProcedureTest.sql"));
            Assert.AreEqual(expectedContent, tableContent);
        }
        [Test(Description = "Процедура удаления")]
        public void DeleteProcedureTest()
        {
            SimpleInit();

            _helper.Construct(_properties, "../..");
            string tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../MainTableDelete.sql"));
            string expectedContent = Utils.ReplaceSpaces(File.ReadAllText("../../ProcedureConstructorHelerTest/DeleteProcedureTest.sql"));
            Assert.AreEqual(expectedContent, tableContent);
        }
        [Test(Description = "Процедура поиск по id")]
        public void ByIdProcedureTest()
        {
            SimpleInit();

            ModalViewModel model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "idfile";
            model.Type = "file";
            model.RuDescription = "Поле 3";
            _rootPanel.Children.Add(model);

            model = new ModalViewModel(_rootPanel);
            model.FieldInDb = "Field3";
            model.Type = ControlTypes.Dictionary;
            model.DictionaryType = DictionaryTypes.DictionaryTable;
            model.DictionaryTableName = "ter_NP_dic";
            model.RuDescription = "Поле 4";
            _rootPanel.Children.Add(model);

            _helper.Construct(_properties, "../..");
            string tableContent = Utils.ReplaceSpaces(File.ReadAllText("../../MainTableById.sql"));
            string expectedContent = Utils.ReplaceSpaces(File.ReadAllText("../../ProcedureConstructorHelerTest/ByIdProcedureTest.sql"));
            Assert.AreEqual(expectedContent, tableContent);
        }
    }
}
