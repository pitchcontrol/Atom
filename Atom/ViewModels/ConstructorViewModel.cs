﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using Atom.Annotations;
using Atom.Models;
using Atom.Services;
using Atom.Views;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Clipboard = System.Windows.Clipboard;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Atom.ViewModels
{
    public class ConstructorViewModel : Screen
    {
        private readonly IEventAggregator _aggregator;
        private readonly IWindowManager _windowManager;
        private WebPageBaseViewModel _currentProperty;
        private RootPanel _rootPanel;
        private ObservableCollection<WebPageBaseViewModel> _properties;


        private string _resourceNameSpace;
        private string _resourceFilePath;
        private string _info;
        private string _tableFolderPath;
        private readonly DiskPath _diskPath;

        public RootPanel RootPanel
        {
            get { return _rootPanel; }
            set { _rootPanel = value; }
        }
        public ConstructorViewModel() { }
        //[Microsoft.Practices.Unity.Dependency]
        //public ShellViewModel Shell { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ConstructorViewModel(IEventAggregator aggregator, IWindowManager windowManager)
        {
            _aggregator = aggregator;
            _windowManager = windowManager;
            Properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(Properties);
            Properties.Add(_rootPanel);

            _diskPath = new DiskPath();
            _diskPath.Add("SaveProject", new PathDialog("json (*.json)|*.json") { Description = "Сохранить проект" })
                .Add("LoadProject",
                    new PathDialog("json (*.json)|*.json")
                    {
                        DefaultPathName = "defaultProjectDir",
                        Description = "Загрузить проект",
                        OpenDialog = true
                    })
                .Add("BuildTables",
                    new PathDialog() { IsFolder = true, Description = "Расположение таблицы", Cache = true })
                .Add("LoadDocument",
                    new PathDialog("resx(*.docx) | *.docx") { Description = "Загрузить ТЗ", OpenDialog = true })
                .Add("BuildProcedures",
                    new PathDialog() { IsFolder = true, Description = "Расположение процедур", Cache = true })
                .Add("ViewPage", new PathDialog("resx(*.aspx) | *.aspx") { Description = "Расположение страницы View", Cache = true })
                .Add("EditPage", new PathDialog("resx(*.aspx) | *.aspx") { Description = "Расположение страницы Edit", Cache = true });
            DisplayName = "Конструктор страницы";
        }

        protected override void OnActivate()
        {
            TryLoadLastProject();
        }

        private void TryLoadLastProject()
        {
            try
            {
                if (!File.Exists("./lastProject.path"))
                    return;
                string path = File.ReadAllText("./lastProject.path");
                if (!File.Exists(path))
                    return;
                LoadFromFile(path);
                _aggregator.PublishOnUIThread($"[Инфо]:Загружен файл {path}\n");
            }
            catch (Exception exception)
            {
                _aggregator.PublishOnUIThread("[Ошибка]:" + exception + "\n");
            }
        }
        /// <summary>
        /// Сохранить проект
        /// </summary>
        public void SaveProject()
        {
            if (_diskPath.GetPath("SaveProject"))
            {
                string json = JsonConvert.SerializeObject(Properties, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                File.WriteAllText(_diskPath.Path, json);
            }
            else
                _aggregator.PublishOnUIThread("[Инфо]:Отказ сохранения\n");
        }
        /// <summary>
        ///Загрузить проект
        /// </summary>
        public void LoadProject()
        {
            if (_diskPath.GetPath("LoadProject"))
            {
                try
                {
                    LoadFromFile(_diskPath.Path);
                    File.WriteAllText("./lastProject.path", _diskPath.Path);
                }
                catch (Exception exception)
                {
                    _aggregator.PublishOnUIThread("[Ошибка]:" + exception + "\n");
                }
            }
            else
            {
                _aggregator.PublishOnUIThread("[Инфо]:Не выбран документ");
            }
        }

        private void LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            var pr = JsonConvert.DeserializeObject<ObservableCollection<WebPageBaseViewModel>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            Properties.Clear();
            foreach (WebPageBaseViewModel webPageBaseViewModel in pr)
            {
                if (webPageBaseViewModel is RootPanel)
                    RootPanel = (RootPanel)webPageBaseViewModel;
                Properties.Add(webPageBaseViewModel);
            }
            SetParentsCollections(RootPanel);
        }

        private void SetParentsCollections(WebPageBaseViewModel parent)
        {
            foreach (WebPageBaseViewModel webPageBaseViewModel in parent.Children)
            {
                //webPageBaseViewModel.ParentCollection = parent.Children;
                webPageBaseViewModel.Parent = parent;
                SetParentsCollections(webPageBaseViewModel);
            }
        }
        /// <summary>
        /// Построить процедуры
        /// </summary>
        public void BuildProcedures()
        {
            if (_diskPath.GetPath("BuildProcedures"))
            {
                ProcedureConstructorHeler helper = new ProcedureConstructorHeler();
                helper.Construct(Properties, _diskPath.Path);
            }
            else
            {
                _aggregator.PublishOnUIThread("[Инфо]:Папка не выбранна");
            }
        }
        /// <summary>
        /// Контролы на странице
        /// </summary>
        public ObservableCollection<WebPageBaseViewModel> Properties
        {
            get { return _properties; }
            private set
            {
                if (value == _properties) return;
                _properties = value;
                NotifyOfPropertyChange();
            }
        }
        /// <summary>
        /// Выбранный контрол в дереве
        /// </summary>
        public WebPageBaseViewModel CurrentProperty
        {
            get { return _currentProperty; }
            set
            {
                if (value == _currentProperty) return;
                _currentProperty = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(CanEditProperty));
                NotifyOfPropertyChange(nameof(CanTurnGrid));
            }
        }
        /// <summary>
        /// Редактирование свойства
        /// </summary>
        public bool CanEditProperty
        {
            get { return CurrentProperty != null; }
        }
        public void EditProperty()
        {
            if (CurrentProperty == null)
                return;
            ModalView window = new ModalView();
            window.DataContext = CurrentProperty;
            if (window.ShowDialog() == true)
            {
            }
        }
        /// <summary>
        /// Получить код страницы
        /// </summary>
        /// <param name="parametr"></param>
        public void GetPage(string parametr)
        {
            if (!EnterResourcePath())
                return;
            PageConstructotHelper helper = new PageConstructotHelper();
            helper.ResourceNamespace = ResourceNameSpace;
            switch (parametr)
            {
                case "e":
                    helper.Construct(Properties, true);
                    Clipboard.SetText(helper.ToString());
                    _aggregator.PublishOnUIThread("[Инфо]:Скопированно в буфер");
                    return;
                case "v":
                    helper.Construct(Properties, true);
                    Clipboard.SetText(helper.ToString());
                    _aggregator.PublishOnUIThread("[Инфо]:Скопированно в буфер");
                    return;
                case "ef":
                    helper.Construct(Properties, true);
                    helper.FullPage = true;
                    if (_diskPath.GetPath("EditPage"))
                    {
                        string file = _diskPath.Path;
                        string csfile = file + ".cs";
                        string designerfile = file + ".designer.cs";
                        File.WriteAllText(file, helper.ToString());
                        File.WriteAllText(csfile, helper.GetCodebehind(true));
                        File.WriteAllText(designerfile, "");
                        _aggregator.PublishOnUIThread("[Инфо]:Записан файл");
                    }

                    return;
                case "vf":
                    if (_diskPath.GetPath("EditPage"))
                    {
                        EnterNameViewModel model = EnterNameViewModel.CreateEnterNamespace();
                        //ModalView window = new ModalView { DataContext = model };
                        if (_windowManager.ShowDialog(model) == true && model.IsValid)
                        {
                            helper.ClassName = model.Value;
                            helper.FullPage = true;
                            helper.Construct(Properties, false);
                            string file = _diskPath.Path;
                            string csfile = file + ".cs";
                            string designerfile = file + ".designer.cs";
                            File.WriteAllText(file, helper.ToString());
                            File.WriteAllText(csfile, helper.GetCodebehind(false));
                            File.WriteAllText(designerfile, "");
                            _aggregator.PublishOnUIThread("[Инфо]:Записан файл");
                        }
                    }
                    return;
            }


        }
        /// <summary>
        /// Установить всем
        /// </summary>
        public void SetAll()
        {
            MassiveSetViewModel model = new MassiveSetViewModel(Properties);
            ModalView window = new ModalView { DataContext = model };
            if (window.ShowDialog() == true)
            {
            }
        }
        /// <summary>
        /// Выбрать расположения файла ресурсов
        /// </summary>
        public void SetResourseFile()
        {
            EnterResourcePath();
        }
        public bool EnterResourcePath()
        {
            if (string.IsNullOrEmpty(ResourceFilePath) || !File.Exists(ResourceFilePath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "resx (*.resx)|*.resx",
                    Title = "Выбрать файл ресурсов",
                    InitialDirectory = ConfigurationManager.AppSettings["defaultResourceDir"]
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    ResourceFilePath = openFileDialog.FileName;
                    int index = openFileDialog.FileName.IndexOf("App_GlobalResources");
                    string subString = openFileDialog.FileName;
                    if (index != -1)
                        subString = openFileDialog.FileName.Substring(index + "App_GlobalResources".Length);
                    else
                    {
                        string parent = Directory.GetDirectoryRoot(ResourceFilePath);
                        subString = subString.Remove(0, parent.Length);
                    }


                    ResourceNameSpace =
                        subString.Replace(@"\", ".").TrimStart(new[] { '.' }).Replace(".resx", "");
                    return true;
                }
                else
                {
                    _aggregator.PublishOnUIThread("[Инфо]:Файл не выбран\n");
                    return false;
                }
            }
            return true;
        }

        public void BuildTables()
        {
            if (_diskPath.GetPath("BuildTables"))
            {
                TableConstructorHelper helper = new TableConstructorHelper();
                helper.ParentTableIdName = _rootPanel.ParentTableId;
                helper.Construct(Properties, _diskPath.Path);
                _aggregator.PublishOnUIThread("[Инфо]:Созданны таблицы\n");
            }
            else
            {
                _aggregator.PublishOnUIThread("[Инфо]:Папка не выбрана\n");
            }
        }
        /// <summary>
        /// Записываем файл ресурсов
        /// </summary>
        /// <returns></returns>
        public bool CanWriteResourses
        {
            get { return _rootPanel.Children.Count > 0; }
        }
        public void WriteResourses()
        {
            if (!EnterResourcePath())
                return;

            WriteResource(ResourceFilePath, false);
            WriteResource(ResourceFilePath.Replace(".resx", ".ru-RU.resx"), true);

        }
        private void WriteResource(string path, bool ru)
        {
            using (ResXResourceReader reader = new ResXResourceReader(path))
            {
                var dictionary = reader.Cast<DictionaryEntry>();
                using (ResXResourceWriter writer = new ResXResourceWriter(path))
                {
                    foreach (DictionaryEntry dictionaryEntry in dictionary)
                        writer.AddResource(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                    foreach (WebPageBaseViewModel webPageBaseViewModel in RootPanel.Children.Flatten(i => i.Children))
                    {
                        writer.AddResource(webPageBaseViewModel.FieldInDb, ru ? webPageBaseViewModel.RuDescription : webPageBaseViewModel.EnDescription);
                    }
                    writer.Generate();
                }
            }
        }
        /// <summary>
        /// Добавить свойство
        /// </summary>
        public void AddProperty()
        {
            ModalView window = new ModalView();
            ModalViewModel model = new ModalViewModel(_rootPanel);
            window.DataContext = model;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(model);
                NotifyOfPropertyChange(nameof(CanWriteResourses));
            }
        }

        /// <summary>
        /// Добавить панель
        /// </summary>
        public void AddPanel()
        {
            ModalView window = new ModalView();
            PanelViewModel panelViewModel = new PanelViewModel(_rootPanel);
            window.DataContext = panelViewModel;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(panelViewModel);
            }
        }
        /// <summary>
        /// Добавить таблицу
        /// </summary>
        public void AddGrid()
        {
            ModalView window = new ModalView();
            GridViewModel panelViewModel = new GridViewModel(_rootPanel);
            window.DataContext = panelViewModel;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(panelViewModel);
            }
        }




        /// <summary>
        /// Преобразовать в грид
        /// </summary>
        public bool CanTurnGrid
        {
            get { return CurrentProperty != null && CurrentProperty.Type == "panel"; }
        }
        public void TurnGrid()
        {
            //Определяем положения в родительской коллекции
            var oldProperty = CurrentProperty;
            var parent = oldProperty.ParentCollection;
            int index = parent.IndexOf(CurrentProperty);
            //Удаляем панель
            parent.RemoveAt(index);
            GridViewModel model = new GridViewModel(oldProperty.Parent)
            {
                FieldInDb = oldProperty.FieldInDb,
                RuDescription = oldProperty.RuDescription,
                EnDescription = oldProperty.EnDescription
            };
            parent.Insert(index, model);
            oldProperty.Children.ForEach(i =>
            {
                //i.ParentCollection = model.Children;
                i.Parent = model;
                model.Children.Add(i);
            });
            oldProperty.Children.Clear();
            CurrentProperty = model;
        }


        /// <summary>
        /// Папка расположения таблиц
        /// </summary>
        public string TableFolderPath
        {
            get { return _tableFolderPath; }
            set
            {
                if (value == _tableFolderPath) return;
                _tableFolderPath = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Путь к файлу ресурсов
        /// </summary>
        public string ResourceFilePath
        {
            get { return _resourceFilePath; }
            set
            {
                if (value == _resourceFilePath) return;
                _resourceFilePath = value;
                NotifyOfPropertyChange();
            }
        }
        /// <summary>
        /// Найм спейс ресурсов
        /// </summary>
        public string ResourceNameSpace
        {
            get { return _resourceNameSpace; }
            set
            {
                if (value == _resourceNameSpace) return;
                _resourceNameSpace = value;
                NotifyOfPropertyChange();
            }
        }
        /// <summary>
        /// Загрузить из ворда
        /// </summary>
        public void LoadFromDocument()
        {
            if (_diskPath.GetPath("LoadDocument"))
            {
                DocumentViewModel model = new DocumentViewModel();
                model.Load(_diskPath.Path);
                Views.DocumentView view = new Views.DocumentView { DataContext = model };
                if (view.ShowDialog() == true)
                {
                    model.Build(Properties);

                    NotifyOfPropertyChange(nameof(CanWriteResourses));
                    _aggregator.PublishOnUIThread("[Инфо]:Загруженно ТЗ");
                }
            }
            else
            {
                _aggregator.PublishOnUIThread("[Инфо]:Не выбран документ");
            }
        }

    }
}
