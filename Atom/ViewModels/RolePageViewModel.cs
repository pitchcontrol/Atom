using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Atom.Models;
using Atom.Services;
using Caliburn.Micro;

namespace Atom.ViewModels
{
    public class RolePageViewModel : Screen
    {
        readonly Dal _dal;
        private readonly IEventAggregator _aggregator;
        private string _page;
        private MenuTree _currentMenuPageView;
        private IEnumerable<MenuTree> _menuGroupViews;
        private IEnumerable<Role> _rolesForPage;
        private string _rolesStr;
        private readonly ConstructorViewModel _constructorViewModel;


        public RolePageViewModel(Dal dal, IEventAggregator aggregator, ConstructorViewModel constructorViewModel)
        {
            _dal = dal;
            _aggregator = aggregator;
            _constructorViewModel = constructorViewModel;
        }

        public override string DisplayName { get { return "Роли страницы страницы"; } }

        public IEnumerable<MenuTree> MenuGroupViews
        {
            get { return _menuGroupViews; }
            set
            {
                if (value == _menuGroupViews) return;
                _menuGroupViews = value;
                NotifyOfPropertyChange();
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
                NotifyOfPropertyChange();
            }
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
                NotifyOfPropertyChange();
            }
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
                NotifyOfPropertyChange();
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
                ScriptConstructorHelper helper = new ScriptConstructorHelper { Visability = 3 };
                helper.Constructor(_constructorViewModel.Properties, isEdit, CurrentMenuPageView.Id, model.SelectRoles.Select(i => i.Id));
                Clipboard.SetText(helper.ToString());
                _aggregator.PublishOnUIThread("[Инфо]:Скопированно в буфер");
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
                NotifyOfPropertyChange();
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

    }
}
