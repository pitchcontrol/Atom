using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.ViewModels;
using NUnit.Framework;

namespace Atom.UnitTests
{
    [TestFixture]
    public class DocumentViewModelTest
    {
        private DocumentViewModel _model;
        [SetUp]
        public void Init()
        {
            _model = new DocumentViewModel();
            _model.Load("../../Test.docx");
        }
        [Test]
        public void TableCountTest()
        {
            Assert.AreEqual(4, _model.TableCount);
        }
        [Test]
        public void HeaderTest()
        {
            _model.CurrentTable = 0;
            Assert.AreEqual(10, _model.Headers.Count());
        }

        [Test]
        public void GetDescritionsTest()
        {
            _model.CurrentTable = 0;
            _model.DescriptionField = _model.Headers.ElementAt(4);
            var descriptions = _model.GetDescriptions();
            int count = descriptions.Count();
            Assert.AreEqual(46, count);
            Assert.AreEqual("Регистрационный номер корпоративного мероприятия", descriptions.ElementAt(0));
            Assert.AreEqual("Дата проведения", descriptions.ElementAt(1));
        }
        [Test]
        public void ValidateTest()
        {
            _model.CurrentTable = 10;
            Assert.IsFalse(_model.IsValid);
        }
    }
}
