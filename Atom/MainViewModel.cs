using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Atom.Commands;
using Atom.Models;
using Atom.Services;
using Atom.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Atom
{
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly atomEntities _context = new atomEntities();
        readonly Dal _dal = new Dal();

        private string _page;
        private IEnumerable<ut_MenuPageView> _pageViews;
        private MenuTree _currentMenuPageView;
        private IEnumerable<int> _roles;
        private IEnumerable<ut_MenuField> _menuFields;
        private IEnumerable<ut_Roles> _utRoleses;
        private string _rolesStr;
        private ut_MenuField _currentMenuField;
        private string _script;
        private string _pageText;
        private string _resuorseTextRu;
        private string _resuorseTextEn;
        private WebPageBaseViewModel _currentProperty;
        private ObservableCollection<ut_Roles> _selectedRole;
        private RootPanel _rootPanel;
        private ObservableCollection<WebPageBaseViewModel> _properties;
        private IEnumerable<MenuTree> _menuGroupViews;
        private IEnumerable<Role> _rolesForPage;
        private string _resourceNameSpace;
        private string _resourceFilePath;

        public RootPanel RootPanel
        {
            get { return _rootPanel; }
            set { _rootPanel = value; }
        }
        public string Page
        {
            get { return _page; }
            set
            {
                if (value == _page) return;
                _page = value;
                OnPropertyChanged();
                if (string.IsNullOrEmpty(_page))
                {
                    //При пустом значении сброс
                    MenuGroupViews = _dal.GetMenuTree();
                }
                else
                {
                    int id;
                    if (int.TryParse(Page, out id))
                    {
                        MenuGroupViews = _dal.GetMenuTree(id);
                        //  PageViews = _context.ut_MenuPageView.Where(i => i.idmenupage == id).ToList();
                    }
                    else
                    {
                        MenuGroupViews = _dal.GetMenuTree(_page);
                        //PageViews = _context.ut_MenuPageView.Where(i => i.nam.Contains(Page)).ToList();
                    }
                }
            }
        }

        public MainViewModel()
        {
            GetScriptCommand = new DelegateCommand<string>(GetScript, (obj) => SelectedRole.Count != 0 && Properties.Count() != 0 && CurrentMenuPageView != null);
            AddPropertyCommand = new DelegateCommand<string>(AddProperty, null);
            ViewPageCommand = new DelegateCommand<string>(GetViewPage, null);
            EditPageCommand = new DelegateCommand<string>(GetEditPage, null);
            GetGridScriptCommand = new DelegateCommand<string>(GetGrid, null);
            EditPropertyCommand = new DelegateCommand<string>(EditProperty, (obj) => CurrentProperty != null);

            AddPanelCommand = new NotifyCommand<MainViewModel>(this, new string[0], AddPanel, (m) => true);
            GetUtMenuPageViewCommand = new SimlpleCommand(() =>
            {
                PageViews = _context.ut_MenuPageView.ToList();
                MenuGroupViews = _dal.GetMenuTree();

            }, null);
            GetGlobalRoles = new SimlpleCommand(() => GlobalRoles = _context.ut_Roles.ToList(), null);
            SelectedRole = new ObservableCollection<ut_Roles>();
            SelectedRole.CollectionChanged += (s, e) => GetScriptCommand.RaiseCanExecuteChanged();
            //PageViews = _context.ut_MenuPageView.ToList();
            //GlobalRoles = _context.ut_Roles.ToList();
            Properties = new ObservableCollection<WebPageBaseViewModel>();
            _rootPanel = new RootPanel(Properties);
            Properties.Add(_rootPanel);

            RuResourceCommand = new GetResourceCommand(this);
            EnResourceCommand = RuResourceCommand;
            GetResourceFileCommand = new GetResourceFileCommand(this);
            SaveCommand = LoadCommand = new StoryObjectCommand(this);
        }
        public SimlpleCommand GetGlobalRoles { get; set; }
        public SimlpleCommand GetUtMenuPageViewCommand { get; set; }
        public DelegateCommand<string> GetScriptCommand { get; set; }
        public DelegateCommand<string> AddPropertyCommand { get; set; }
        public DelegateCommand<string> ViewPageCommand { get; set; }
        public DelegateCommand<string> EditPageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public DelegateCommand<string> EditPropertyCommand { get; set; }
        public ICommand RuResourceCommand { get; set; }
        public ICommand GetResourceFileCommand { get; set; }
        public ICommand EnResourceCommand { get; set; }
        public ICommand AddPanelCommand { get; private set; }
        public DelegateCommand<string> GetGridScriptCommand { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public string Script
        {
            get { return _script; }
            set
            {
                if (value == _script) return;
                _script = value;
                OnPropertyChanged();
            }
        }
        public string PageText
        {
            get { return _pageText; }
            set
            {
                if (value == _pageText) return;
                _pageText = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Выбранные роли
        /// </summary>
        public ObservableCollection<ut_Roles> SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                if (value == _selectedRole) return;
                _selectedRole = value;
                OnPropertyChanged();
                GetScriptCommand.RaiseCanExecuteChanged();
            }
        }
        /// <summary>
        /// Ресурсы на русском
        /// </summary>
        public string ResuorseTextRu
        {
            get { return _resuorseTextRu; }
            set
            {
                if (value == _resuorseTextRu) return;
                _resuorseTextRu = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Ресурсы на англ.
        /// </summary>
        public string ResuorseTextEn
        {
            get { return _resuorseTextEn; }
            set
            {
                if (value == _resuorseTextEn) return;
                _resuorseTextEn = value;
                OnPropertyChanged();
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
                GetScriptCommand.RaiseCanExecuteChanged();
                //EnResourceCommand.RaiseCanExecuteChanged();
                // RuResourceCommand.RaiseCanExecuteChanged();
            }
        }

        public WebPageBaseViewModel CurrentProperty
        {
            get { return _currentProperty; }
            set
            {
                if (value == _currentProperty) return;
                _currentProperty = value;
                OnPropertyChanged();
                EditPropertyCommand.RaiseCanExecuteChanged();
            }
        }

        private void EditProperty(string obj)
        {
            ModalView window = new ModalView();
            window.DataContext = CurrentProperty;
            if (window.ShowDialog() == true)
            {
            }
        }

        /// <summary>
        /// Получить страницу View
        /// </summary>
        /// <param name="obj"></param>
        private void GetViewPage(string obj)
        {
            PageConstructotHelper helper = new PageConstructotHelper();
            helper.Construct(Properties, false);
            PageText = helper.ToString();
        }

        private void AddProperty(string obj)
        {
            ModalView window = new ModalView();
            ModalViewModel model = new ModalViewModel(_rootPanel.Children);
            window.DataContext = model;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(model);
                OnPropertyChanged("Properties");
            }
        }
        private void AddPanel(MainViewModel model)
        {
            ModalView window = new ModalView();
            Panel panel = new Panel(_rootPanel.Children);
            window.DataContext = panel;
            if (window.ShowDialog() == true)
            {
                _rootPanel.Children.Add(panel);
                OnPropertyChanged("Properties");
            }

        }

        private void GetGrid(string obj)
        {
            GridConstructorHelper helper = new GridConstructorHelper();
            helper.Construct(Properties);
            PageText = helper.ToString();
        }
        private void GetEditPage(string obj)
        {
            PageConstructotHelper helper = new PageConstructotHelper();
            helper.Construct(Properties, true);
            PageText = helper.ToString();
        }
        /// <summary>
        /// Получить скрипт
        /// </summary>
        /// <param name="obj"></param>
        private void GetScript(string obj)
        {
            ScriptConstructorHelper helper = new ScriptConstructorHelper();
            helper.Constructor(Properties, false, (int)CurrentMenuPageView.Id, SelectedRole.Select(i => i.pkid));
            Script = helper.ToString();
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

        public IEnumerable<ut_MenuPageView> PageViews
        {
            get { return _pageViews; }
            set
            {
                if (value == _pageViews) return;
                _pageViews = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<ut_MenuField> MenuFields
        {
            get { return _menuFields; }
            set
            {
                if (value == _menuFields) return;
                _menuFields = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ut_Roles> GlobalRoles
        {
            get { return _utRoleses; }
            set
            {
                if (value == _utRoleses) return;
                _utRoleses = value;
                OnPropertyChanged();
            }
        }

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

        public string DataBase
        {
            get { return ConnectionService.GetConnectionString(); }
        }

        public string ResourceNameSpace
        {
            get { return _resourceNameSpace; }
            set
            {
                if (value == _resourceNameSpace) return;
                _resourceNameSpace = value;
                OnPropertyChanged();
                _resourceNameSpace = value;
            }
        }

        public ut_MenuField CurrentMenuField
        {
            get { return _currentMenuField; }
            set
            {
                if (value == _currentMenuField) return;
                _currentMenuField = value;
                OnPropertyChanged();
                if (_currentMenuField != null)
                {
                    RolesStr = _currentMenuField.ut_RoleField.Aggregate("", (a, c) => a + "r=" + c.idrole + ", v=" + c.visability + "; ", (a) => a.TrimEnd(new char[] { ';', ' ' }));
                }
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
                if (_currentMenuPageView != null && !_currentMenuPageView.IsGroup)
                    MenuFields = _context.ut_MenuField.Where(i => i.idpage == _currentMenuPageView.Id).ToList();
                else
                    MenuFields = null;
                if (_currentMenuPageView != null)
                {
                    RolesForPage = _currentMenuPageView.IsGroup ? _dal.GetRoleForGroup(_currentMenuPageView.Id) : _dal.GetRoleForPage(_currentMenuPageView.PageId);
                }
                GetScriptCommand.RaiseCanExecuteChanged();
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


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            GetScriptCommand.RaiseCanExecuteChanged();
        }
    }
}
