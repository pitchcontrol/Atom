using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using Atom.Constant;
using Atom.Models;
using Atom.Properties;
using Atom.ViewModels;

namespace Atom.Services
{
    public class PageConstructotHelper
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();
        /// <summary>
        /// Расположени ресурсов
        /// </summary>
        public string ResourceNamespace { get; set; }
        /// <summary>
        /// Полная страница
        /// </summary>
        public bool FullPage { get; set; }
        /// <summary>
        /// Имя класса с наймспейсом
        /// </summary>
        public string ClassName { get; set; }
        public string Codebehind { get; set; }

        public void Construct(IEnumerable<WebPageBaseViewModel> collection, bool isEdit)
        {
            foreach (WebPageBaseViewModel modalViewModel in collection)
            {
                if (isEdit && !modalViewModel.IsEditable)
                    continue;
                string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdView);
                if (modalViewModel is PanelViewModel)
                {
                    _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
                    _stringBuilder.AppendFormat(
                        "<gp:CollapsePanel ID=\"{0}\" runat=\"server\" Caption=\"{1}\" SkinID=\"CollapsePanel\">\n",
                        isEdit ? modalViewModel.ControlIdEdit : modalViewModel.ControlIdView, caption);
                }
                else if (modalViewModel is GridViewModel)
                {
                    _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
                    _stringBuilder.AppendFormat("<gp:GpGridView ID=\"gvObject\" Height=\"100%\" runat=\"server\" PageSize=\"10\" ShowWhenEmpty=\"False\" SkinID=\"gpGridView\" DataKeyNames=\"id\" PagerSettings-Visible=\"true\">\n");
                    _stringBuilder.AppendFormat("<Columns>\n");
                }
                else
                {
                    if (isEdit)
                    {
                        WriteEdit(modalViewModel);
                    }
                    else
                    {
                        WriteView(modalViewModel);
                    }
                }
                if (modalViewModel is GridViewModel)
                    WriteGrid(modalViewModel.Children);
                else
                    Construct(modalViewModel.Children, isEdit);
                if (modalViewModel is PanelViewModel)
                {
                    _stringBuilder.AppendFormat(
                        "</gp:CollapsePanel>\n");
                }
                if (modalViewModel is GridViewModel)
                {
                    _stringBuilder.AppendFormat("</Columns>\n");
                    _stringBuilder.AppendFormat("</gp:GpGridView>\n");
                }
            }
        }
        /// <summary>
        /// Создает контрол в режиме View
        /// </summary>
        /// <param name="modalViewModel">Обьект поле</param>
        private void WriteView(WebPageBaseViewModel modalViewModel)
        {
            _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
            string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdView);
            switch (modalViewModel.Type)
            {
                case "int":
                case "varchar":
                case "datetime":
                case "decimal":
                case "dictionary":
                    _stringBuilder.AppendFormat("<gp:ValidatingLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "bit":
                    _stringBuilder.AppendFormat("<gp:ValidatingBoolLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "file":
                    _stringBuilder.AppendFormat("<gp:ValidatingFileView ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "time":
                    _stringBuilder.AppendFormat("<gp:ValidatingLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\" Format=\"HH:mm:ss\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "date":
                    _stringBuilder.AppendFormat("<gp:ValidatingLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\" Format=\"MM.dd.yyyy\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
            }
        }
        /// <summary>
        /// Добавить грид
        /// </summary>
        /// <param name="modalViewModel"></param>
        private void WriteGrid(IEnumerable<WebPageBaseViewModel> collection)
        {
            foreach (var modalViewModel in collection)
            {
                _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
                string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.FieldInDb);
                _stringBuilder.AppendFormat("<asp:TemplateField HeaderText=\"{0}\" SortExpression=\"{1}\" AccessibleHeaderText=\"{1}\">\n", caption, modalViewModel.FieldInDb);
                _stringBuilder.AppendFormat("<ItemTemplate>\n");
                switch (modalViewModel.Type)
                {
                    case "int":
                    case "varchar":
                    case "datetime":
                    case "decimal":
                    case "dictionary":
                        _stringBuilder.AppendFormat("<asp:Label ID=\"{0}\" runat=\"server\"><%# Eval(\"{1}\")%></asp:Label>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "bit":
                        _stringBuilder.AppendFormat("<asp:CheckBox  ID=\"{0}\" runat=\"server\" value=\"<%# Eval(\"{1}\")%>\"/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "file":
                        _stringBuilder.AppendFormat("<gp:ValidatingFileView ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" FileName=\"<%# Eval(\"fileName\")%>\" Value='<%# (Eval(\"{1}\") is DBNull) ? -1 : Eval(\"{1}\") %>'/>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "date":
                        _stringBuilder.AppendFormat("<asp:Label ID=\"{0}\" runat=\"server\"><%# Eval(\"{1}\",\"MM.dd.yyyy\")%></asp:Label>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                    case "time":
                        _stringBuilder.AppendFormat("<asp:Label ID=\"{0}\" runat=\"server\"><%# Eval(\"{1}\",\"HH:mm:ss\")%></asp:Label>\n",
                            modalViewModel.ControlIdView,
                            modalViewModel.FieldInDb);
                        break;
                }
                _stringBuilder.AppendFormat("</ItemTemplate>\n");
                _stringBuilder.AppendFormat("</asp:TemplateField>\n");
            }
        }
        /// <summary>
        /// Создает контрол в режиме Edit
        /// </summary>
        /// <param name="modalViewModel">Обьект поле</param>
        private void WriteEdit(WebPageBaseViewModel modalViewModel)
        {
            _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
            string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdEdit);
            switch (modalViewModel.Type)
            {
                case "time":
                case "date":
                case "datetime":
                    _stringBuilder.AppendFormat("<gp:ValidatingJsCalendar ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\" ImageUrl=\"~/Images/week_small.gif\"  ValidType=\"FORM_ERROR_TYPE_DATE\" />\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "int":
                    _stringBuilder.AppendFormat("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" sqlType=\"Int\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                    modalViewModel.ControlIdView,
                    modalViewModel.FieldInDb, caption);
                    break;
                case "decimal":
                    _stringBuilder.AppendFormat("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" sqlType=\"Decimal\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                    modalViewModel.ControlIdView,
                    modalViewModel.FieldInDb, caption);
                    break;
                case "varchar":
                    _stringBuilder.AppendFormat("<gp:ValidatingTextBox ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "bit":
                    _stringBuilder.AppendFormat("<gp:ValidatingBoolLabel ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "file":
                    _stringBuilder.AppendFormat("<gp:ValidatingFileView ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;
                case "dictionary":
                    _stringBuilder.AppendFormat("<gp:ValidatingDropDawnList ID=\"{0}\" runat=\"server\" SkinID=\"ViewModeSkin\" Caption=\"{2}\" DataBoundField=\"{1}\" EnableDate=\"true\" HistType=\"HISTORY_TYPE_UL\"/>\n",
                        modalViewModel.ControlIdView,
                        modalViewModel.FieldInDb, caption);
                    break;

            }
        }

        public string GetCodebehind(bool isEdit)
        {
            string template = isEdit ? Resources.PageEdit_cs:Resources.PageView_cs;
            string className = ClassName.Split('.').LastOrDefault();

            return template.Replace(Templates.ShortClassName, className);
        }
        public override string ToString()
        {
            if (FullPage)
            {
                string template = Resources.PageView;
                return
                    template.Replace(Templates.Content, _stringBuilder.ToString())
                        .Replace(Templates.ClassName, ClassName)
                        .Replace(Templates.Codebehind, Codebehind);
            }
            return _stringBuilder.ToString();
        }
    }
}
