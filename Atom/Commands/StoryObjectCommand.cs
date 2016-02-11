using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Atom.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Atom.Commands
{
    public class StoryObjectCommand : ICommand
    {
        private readonly ConstructorViewModel _model;

        public StoryObjectCommand(ConstructorViewModel model)
        {
            _model = model;
            _model.PropertyChanged += (s, e) =>
            {
                if (CanExecuteChanged != null && e.PropertyName == "Properties")
                    CanExecuteChanged(this, e);
            };
        }

        public bool CanExecute(object parameter)
        {
            bool isLoad = parameter.ToString() == "True";
            return isLoad || (_model.Properties.Count != 0 && _model.Properties.FirstOrDefault().Children.Count != 0);
        }

        public void Execute(object parameter)
        {
            bool isLoad = parameter.ToString() == "True";
            if (isLoad)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "json (*.json)|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string json = File.ReadAllText(openFileDialog.FileName);
                        var pr = JsonConvert.DeserializeObject<ObservableCollection<WebPageBaseViewModel>>(json, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects
                        });

                        _model.Properties.Clear();
                        foreach (WebPageBaseViewModel webPageBaseViewModel in pr)
                        {
                            if (webPageBaseViewModel is RootPanel)
                                _model.RootPanel = (RootPanel) webPageBaseViewModel;
                            _model.Properties.Add(webPageBaseViewModel);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Ошибка");
                    }
                }
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "json (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string json = JsonConvert.SerializeObject(_model.Properties, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    });
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
