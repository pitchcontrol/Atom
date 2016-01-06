﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Atom.Annotations;
using Atom.Commands;
using Atom.Models;
using Atom.Services;
using Atom.ViewModels;
using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Atom
{
    public class ShellViewModel : INotifyPropertyChanged, IShell
    {
        readonly Dal _dal;
        private string _page;
        private MenuTree _currentMenuPageView;
        private string _rolesStr;
        private WebPageBaseViewModel _currentProperty;
        private RootPanel _rootPanel;
        private ObservableCollection<WebPageBaseViewModel> _properties;
        private IEnumerable<MenuTree> _menuGroupViews;
        private IEnumerable<Role> _rolesForPage;
        private string _resourceNameSpace;
        private string _resourceFilePath;
        private string _info;

        public RootPanel RootPanel
        {
            get { return _rootPanel; }
            set { _rootPanel = value; }
        }
        /// <summary>
        /// Страница ipmenupage или nam
        /// </summary>
        public string Page
        {
            get { return _page; }
            set
            {
                if (value == _page) return;
                _page = value;
                OnPropertyChanged();
                if (string.IsNullOrEmpty(_page))
                    //При пустом значении сброс
                    MenuGroupViews = _dal.GetMenuTree();
                else
                {
                    int id;
                    MenuGroupViews = int.TryParse(Page, out id) ? _dal.GetMenuTree(id) : _dal.GetMenuTree(_page);
                }
            }
        }

        public ShellViewModel()
        {
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ShellViewModel(Dal dal)
        {
            _dal = dal;
            Properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(Properties);
            Properties.Add(_rootPanel);
        }
        /// <summary>
        /// Информация
        /// </summary>
        public string Info
        {
            get { return _info; }
            set
            {
                if (value == _info) return;
                _info = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Сохранить проект
        /// </summary>
        public void SaveProject()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "json (*.json)|*.json" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(Properties, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                File.WriteAllText(saveFileDialog.FileName, json);
            }
            else
                Info += "[Инфо]:Отказ сохранения\n";
        }
        /// <summary>
        ///Загрузить проект
        /// </summary>
        public void LoadProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "json (*.json)|*.json", InitialDirectory = ConfigurationManager.AppSettings["defaultProjectDir"] };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
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
                catch (Exception exception)
                {
                    Info += "[Ошибка]:" + exception + "\n";
                }
            }
        }
        private void SetParentsCollections(WebPageBaseViewModel parent)
        {
            foreach (WebPageBaseViewModel webPageBaseViewModel in parent.Children)
            {
                webPageBaseViewModel.ParentCollection = parent.Children;
                SetParentsCollections(webPageBaseViewModel);
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEditProperty));
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
            if (CurrentProperty == null || CurrentProperty is RootPanel)
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
            bool isEdit = parametr == "e";
            if (!EnterResourcePath())
                return;
            PageConstructotHelper helper = new PageConstructotHelper();
            helper.ResourceNamespace = ResourceNameSpace;
            helper.Construct(Properties, isEdit);
            Clipboard.SetText(helper.ToString());
            Info += "[Инфо]:Скопированно в буфер";
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
                    string subString = openFileDialog.FileName.Substring(index + "App_GlobalResources".Length);
                    ResourceNameSpace =
                        subString.Replace(@"\", ".").TrimStart(new[] { '.' }).TrimEnd(new[] { '.', 'r', 'e', 's', 'x' });
                    return true;
                }
                else
                {
                    Info += "[Инфо]:Файл не выбран\n";
                    return false;
                }
            }
            return true;
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
            ModalViewModel model = new ModalViewModel(_rootPanel.Children);
            window.DataContext = model;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(model);
                OnPropertyChanged(nameof(CanWriteResourses));
            }
        }

        /// <summary>
        /// Добавить панель
        /// </summary>
        public void AddPanel()
        {
            ModalView window = new ModalView();
            PanelViewModel panelViewModel = new PanelViewModel(_rootPanel.Children);
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
            GridViewModel panelViewModel = new GridViewModel(_rootPanel.Children);
            window.DataContext = panelViewModel;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(panelViewModel);
            }
        }

        /// <summary>
        /// Получить скрипт для редактирования
        /// </summary>
        public void GetPageScript(string param)
        {
            bool isEdit = param == "e";
            ModalView window = new ModalView();
            RolesViewModel model = new RolesViewModel { Roles = _dal.GetGlobalRoles() };
            window.DataContext = model;
            if (window.ShowDialog() == true)
            {
                ScriptConstructorHelper helper = new ScriptConstructorHelper();
                helper.Visability = 3;
                helper.Constructor(Properties, isEdit, CurrentMenuPageView.Id, model.SelectRoles.Select(i => i.Id));
                Clipboard.SetText(helper.ToString());
                Info += "[Инфо]:Скопированно в буфер";
            }
        }
        public IEnumerable<MenuTree> MenuGroupViews
        {
            get { return _menuGroupViews; }
            set
            {
                if (value == _menuGroupViews) return;
                _menuGroupViews = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Роли для страницы как ид через запятую
        /// </summary>
        public string RolesStr
        {
            get { return _rolesStr; }
            set
            {
                if (value == _rolesStr) return;
                _rolesStr = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранная страница
        /// </summary>
        public MenuTree CurrentMenuPageView
        {
            set
            {
                if (value == _currentMenuPageView) return;
                _currentMenuPageView = value;
                OnPropertyChanged();
                //TODO: Разобратся
                //if (_currentMenuPageView != null && !_currentMenuPageView.IsGroup)
                //    MenuFields = null;
                //else
                //    MenuFields = null;
                if (_currentMenuPageView != null)
                {
                    RolesForPage = _currentMenuPageView.IsGroup ? _dal.GetRoleForGroup(_currentMenuPageView.Id) : _dal.GetRoleForPage(_currentMenuPageView.PageId);
                    RolesStr = string.Join(", ", RolesForPage.Select(i => i.Id));
                }
            }
            get { return _currentMenuPageView; }
        }
        /// <summary>
        /// Роли для выбранной страницы
        /// </summary>
        public IEnumerable<Role> RolesForPage
        {
            get { return _rolesForPage; }
            set
            {
                if (value == _rolesForPage) return;
                _rolesForPage = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Загрузить из ворда
        /// </summary>
        public void LoadFromDocument()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Загрузить ТЗ";
            dialog.Filter = "resx(*.docx) | *.docx";
            if (dialog.ShowDialog() == true)
            {
                DocumentViewModel model = new DocumentViewModel();
                model.Load(dialog.FileName);
                ModalView view = new ModalView { DataContext = model };
                if (view.ShowDialog() == true)
                {
                    //var descriptions = model.GetDescriptions();
                    //descriptions.ForEach((i, c) =>
                    //{
                    //    ModalViewModel field = new ModalViewModel(_rootPanel.Children) { FieldInDb = "fields" + c, RuDescription = i };
                    //    _rootPanel.Children.Add(field);
                    //});
                    //var groups = model.GetGroupNames();
                    //groups.ForEach((i, c) =>
                    //{
                    //    PanelViewModel panel = new PanelViewModel(_rootPanel.Children) { FieldInDb = "fields" + c, RuDescription = i };
                    //    _rootPanel.Children.Add(panel);
                    //});

                    model.Build(Properties);

                    OnPropertyChanged(nameof(CanWriteResourses));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}