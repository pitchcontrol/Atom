using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Atom.Commands
{
    public class GetResourceFileCommand : ICommand
    {
        private ShellViewModel _model;
        public GetResourceFileCommand(ShellViewModel model)
        {
            _model = model;
            _model.Properties.CollectionChanged += (s, e) =>
            {
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
            };
        }

        public bool CanExecute(object parameter)
        {
            return _model.Properties.Count != 0;
        }

        public void Execute(object parameter)
        {
            if (string.IsNullOrEmpty(_model.ResourceFilePath) || !File.Exists(_model.ResourceFilePath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "resx (*.resx)|*.resx";
                if (openFileDialog.ShowDialog() == true)
                {
                    _model.ResourceFilePath = openFileDialog.FileName;
                }
            }
            XDocument doc = XDocument.Load(_model.ResourceFilePath);
            var data = doc.Root.Elements("data");
            var atrib = data.FirstOrDefault().Attributes().ElementAt(1);
            doc.Root.Add(new XElement("data", new XAttribute("name","MyObject"), atrib,
                new XElement("value","MyData")));
            doc.Save(_model.ResourceFilePath);
        }

        public event EventHandler CanExecuteChanged;
    }
}
