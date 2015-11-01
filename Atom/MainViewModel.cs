using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Atom
{
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly atomEntities _context = new atomEntities();

        private string _page;
        private IEnumerable<ut_MenuPageView> _pageViews;
        private ut_MenuPageView _currentMenuPageView;
        private IEnumerable<int> _roles;
        private IEnumerable<ut_MenuField> _menuFields;
        private IEnumerable<ut_Roles> _utRoleses;
        private string _rolesStr;
        private ut_MenuField _currentMenuField;
        private string _script;
        private string _pageText;
        private string _resuorseText;
        private ModalViewModel _currentProperty;
        private ut_Roles _selectedRole;
        private readonly RootPanel _rootPanel;
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
                    //PageViews = _context.ut_MenuPageView.ToList();
                }
                else
                {
                    int id;
                    if (int.TryParse(Page, out id))
                    {
                        PageViews = _context.ut_MenuPageView.Where(i => i.idmenupage == id).ToList();
                    }
                    else
                    {
                        PageViews = _context.ut_MenuPageView.Where(i => i.nam.Contains(Page)).ToList();
                    }
                }
            }
        }

        public MainViewModel()
        {
            GetScriptCommand = new DelegateCommand<string>(GetScript, (obj) => SelectedRole != null && Properties.Count() != 0 && CurrentMenuPageView != null);
            AddPropertyCommand = new DelegateCommand<string>(AddProperty, null);
            ViewPageCommand = new DelegateCommand<string>(GetViewPage, null);
            EditPageCommand = new DelegateCommand<string>(GetEditPage, null);
            SaveCommand = new DelegateCommand<string>(SaveObj, null);
            LoadCommand = new DelegateCommand<string>(LoadObj, null);
            EditPropertyCommand = new DelegateCommand<string>(EditProperty, (obj) => CurrentProperty != null);
            RuResourceCommand = new DelegateCommand<string>(GetRuResource, (onj) => Properties.Count() != 0);
            EnResourceCommand = new DelegateCommand<string>(GetEnResource, (onj) => Properties.Count() != 0);
            //PageViews = _context.ut_MenuPageView.ToList();
            //GlobalRoles = _context.ut_Roles.ToList();
            Properties = new ObservableCollection<WebPageBaseViewModel>();

            _rootPanel= new RootPanel(Properties);
            Properties.Add(_rootPanel);
            //Root
            //ДЛя теста 
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children)
            {
                FieldInDb = "field1"
            });
            _rootPanel.Children.Add(new Panel(_rootPanel.Children)
            {
                FieldInDb = "Panel1"
            });
            _rootPanel.Children.Add(new ModalViewModel(_rootPanel.Children)
            {
                FieldInDb = "field3"
            });
        }

        public DelegateCommand<string> GetScriptCommand { get; set; }
        public DelegateCommand<string> AddPropertyCommand { get; set; }
        public DelegateCommand<string> ViewPageCommand { get; set; }
        public DelegateCommand<string> EditPageCommand { get; set; }
        public DelegateCommand<string> SaveCommand { get; set; }
        public DelegateCommand<string> LoadCommand { get; set; }
        public DelegateCommand<string> EditPropertyCommand { get; set; }
        public DelegateCommand<string> RuResourceCommand { get; set; }
        public DelegateCommand<string> EnResourceCommand { get; set; }

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
        public ut_Roles SelectedRole
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

        public string ResuorseText
        {
            get { return _resuorseText; }
            set
            {
                if (value == _resuorseText) return;
                _resuorseText = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<WebPageBaseViewModel> Properties { get; }

        public ModalViewModel CurrentProperty
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
        private void AddProperty(string obj)
        {
            ModalView window = new ModalView();
            ModalViewModel model = new ModalViewModel(Properties);
            window.DataContext = model;
            if (window.ShowDialog() == true)
            {
                Properties.Add(model);
            }
        }
        /// <summary>
        /// Получить XML ресурса
        /// </summary>
        /// <param name="obj"></param>
        private void GetRuResource(string obj)
        {
            string result = "";
            foreach (ModalViewModel modalViewModel in Properties)
            {
                result += string.Format("<data name=\"{0}\" xml:space=\"preserve\">\n", modalViewModel.FieldInDb);
                result += string.Format("<value>{0}</value>\n</data>\n", modalViewModel.RuDescription);
            }
            ResuorseText = result;
        }
        private void GetEnResource(string obj)
        {
            string result = "";
            foreach (ModalViewModel modalViewModel in Properties)
            {
                result += string.Format("<data name=\"{0}\" xml:space=\"preserve\">\n", modalViewModel.FieldInDb);
                result += string.Format("<value>{0}</value>\n</data>\n", modalViewModel.EnDescription);
            }
            ResuorseText = result;
        }
        /// <summary>
        /// Получить страницу View
        /// </summary>
        /// <param name="obj"></param>
        private void GetViewPage(string obj)
        {
            string result = "";
            foreach (ModalViewModel modalViewModel in Properties)
            {
                result += string.Format("<%--{0}--%>\n", modalViewModel.RuDescription);
                switch (modalViewModel.Type)
                {
                    case "int":
                    case "varchar":
                    case "datetime":
                    case "decimal":
                    case "dictionary":
                        result += string.Format("<gp:ValidatingLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "bit":
                        result += string.Format("<gp:ValidatingBoolLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "file":
                        result += string.Format("<gp:ValidatingFileView ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                }
            }
            PageText = result;
        }

        private void SaveObj(string obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "json (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(Properties, Formatting.Indented);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void LoadObj(string obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var lst = JsonConvert.DeserializeObject<List<ModalViewModel>>(json);
                Properties.Clear();
                lst.ForEach(Properties.Add);
                GetScriptCommand.RaiseCanExecuteChanged();
                EnResourceCommand.RaiseCanExecuteChanged();
                RuResourceCommand.RaiseCanExecuteChanged();
            }
        }
        private void GetEditPage(string obj)
        {
            string result = "";
            foreach (ModalViewModel modalViewModel in Properties)
            {
                result += string.Format("<%--{0}--%>\n", modalViewModel.RuDescription);
                switch (modalViewModel.Type)
                {
                    case "datetime":
                        result += string.Format("<gp:ValidatingJsCalendar ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\" ImageUrl=\"~/Images/week_small.gif\"  ValidType=\"FORM_ERROR_TYPE_DATE\" />\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "int":
                        result += string.Format("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" sqlType=\"Int\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb);
                        break;
                    case "decimal":
                        result += string.Format("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" sqlType=\"Decimal\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb);
                        break;
                    case "varchar":
                        result += string.Format("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "bit":
                        result += string.Format("<gp:ValidatingBoolLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "file":
                        result += string.Format("<gp:ValidatingFileView ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "dictionary":
                        result += string.Format("<gp:ValidatingDropDawnList ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;

                }
            }
            PageText = result;
        }
        /// <summary>
        /// Получить скрипт
        /// </summary>
        /// <param name="obj"></param>
        private void GetScript(string obj)
        {
            string result = "DECLARE @id int;\n";
            foreach (ModalViewModel model in Properties)
            {
                string description = string.Format("ru-RU:{0};en-EN:{1};", model.RuDescription, model.EnDescription);
                result += string.Format("--{0}\n", model.FieldInDb);
                result +=
                    string.Format(
                        "INSERT INTO [ut_MenuField] (idpage,fld, idparent, fldbd, tabbd, isNotEdited, nam) VALUES ({0}, '{1}', null, '{2}', '{3}' , 0, '{4}');\n",
                        CurrentMenuPageView.idmenupage,
                        model.ControlIdView,
                        model.FieldInDb ?? "null",
                        model.TableName ?? "null",
                        description);

                result += "set @id  = scope_identity();\n";
                result += string.Format("insert into [ut_RoleField] (idrole, idfld,visability)\nvalues\n");
                result += string.Format("({0},@id,{1})\n", SelectedRole.pkid, 1);
                //                result = SelectedRole.ut_RoleField.Aggregate(result,
                //                    (current, utRoleField) =>
                //                        current + string.Format("({0},@id,{1}),\n", utRoleField.idrole, 1));
                //                result = result.TrimEnd('\n', ',') + ";";
            }
            Script = result;
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


        public ut_MenuPageView CurrentMenuPageView
        {
            set
            {
                if (value == _currentMenuPageView) return;
                _currentMenuPageView = value;
                OnPropertyChanged();
                if (_currentMenuPageView != null)
                {
                    MenuFields = _context.ut_MenuField.Where(i => i.idpage == _currentMenuPageView.idmenupage).ToList();
                }
                GetScriptCommand.RaiseCanExecuteChanged();
            }
            get { return _currentMenuPageView; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            GetScriptCommand.RaiseCanExecuteChanged();
        }
    }
}
