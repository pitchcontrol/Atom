using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class ResourceTests
    {
        ShellViewModel _model;
        [SetUp]
        public void Init()
        {
            if (!File.Exists("../../ResourceTemp.resx"))
                File.Copy("../../Resource.resx", "../../ResourceTemp.resx");
            if (!File.Exists("../../ResourceTemp.ru-RU.resx"))
                File.Copy("../../Resource.ru-RU.resx", "../../ResourceTemp.ru-RU.resx");
            _model = new ShellViewModel();
            _model.ResourceFilePath = "../../ResourceTemp.resx";
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete("../../ResourceTemp.resx");
            File.Delete("../../ResourceTemp.ru-RU.resx");

        }
        [Test]
        public void AddNewKeyTest()
        {
            PanelViewModel panelViewModel = new PanelViewModel(_model.RootPanel) { RuDescription = "Комент", FieldInDb = "Id1" };
            _model.RootPanel.Children.Add(panelViewModel);
            //Добавляем ключь, должны добавится в оба словаря
            _model.WriteResourses();
            //Проверяем
            ResXResourceReader reader = new ResXResourceReader(_model.ResourceFilePath);
            var dictionary = reader.Cast<DictionaryEntry>();
            Assert.AreEqual(2, dictionary.Count());
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "Id1"));
            reader.Close();

            reader = new ResXResourceReader(_model.ResourceFilePath.Replace(".resx", ".ru-RU.resx"));
            dictionary = reader.Cast<DictionaryEntry>();
            Assert.AreEqual(2, dictionary.Count());
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "Id1"));
            reader.Close();
        }
        [Test]
        public void AddDoubleKey()
        {
            PanelViewModel panelViewModel = new PanelViewModel(_model.RootPanel) { RuDescription = "Комент", FieldInDb = "Key1" };
            _model.RootPanel.Children.Add(panelViewModel);
            //Добавляем ключь, должны добавится в оба словаря
            _model.WriteResourses();
            //Проверяем
            ResXResourceReader reader = new ResXResourceReader(_model.ResourceFilePath);
            var dictionary = reader.Cast<DictionaryEntry>();
            Assert.AreEqual(1, dictionary.Count());
            reader.Close();

            reader = new ResXResourceReader(_model.ResourceFilePath.Replace(".resx", ".ru-RU.resx"));
            dictionary = reader.Cast<DictionaryEntry>();
            Assert.AreEqual(1, dictionary.Count());
            reader.Close();
        }
        [Test]
        public void AddRecursiveKey()
        {
            PanelViewModel panel1 = new PanelViewModel(_model.RootPanel);
            panel1.FieldInDb = "fl1";
            _model.RootPanel.Children.Add(panel1);

            PanelViewModel panel2 = new PanelViewModel(panel1);
            panel2.FieldInDb = "fl2";
            panel1.Children.Add(panel2);

            PanelViewModel panel3 = new PanelViewModel(panel2);
            panel3.FieldInDb = "fl3";
            panel2.Children.Add(panel3);
            //Добавляем ключь, должны добавится в оба словаря
            _model.WriteResourses();
            //Проверяем
            ResXResourceReader reader = new ResXResourceReader(_model.ResourceFilePath);
            var dictionary = reader.Cast<DictionaryEntry>();
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl1"));
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl2"));
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl3"));
            Assert.AreEqual(4, dictionary.Count());
            reader.Close();

            reader = new ResXResourceReader(_model.ResourceFilePath.Replace(".resx", ".ru-RU.resx"));
            dictionary = reader.Cast<DictionaryEntry>();
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl1"));
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl2"));
            Assert.IsNotNull(dictionary.FirstOrDefault(i => i.Key == "fl3"));
            Assert.AreEqual(4, dictionary.Count());
            reader.Close();
        }
    }
}
