using System.Collections.Generic;
using System.Text;

namespace Atom.Services
{
    public class PageConstructotHelper
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();
        /// <summary>
        /// Расположени ресурсов
        /// </summary>
        public string ResourceNamespace { get; set; }

        public void Construct(IEnumerable<WebPageBaseViewModel> collection, bool isEdit)
        {
            foreach (WebPageBaseViewModel modalViewModel in collection)
            {
                if (isEdit && !modalViewModel.IsEditable)
                    continue;
                string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdView);
                if (modalViewModel is Panel)
                {
                    _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
                    _stringBuilder.AppendFormat(
                        "<gp:CollapsePanel ID=\"{0}\" runat=\"server\" Caption=\"{1}\" SkinID=\"CollapsePanel\">\n",
                        isEdit ? modalViewModel.ControlIdEdit : modalViewModel.ControlIdView, caption);
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
                Construct(modalViewModel.Children, isEdit);
                if (modalViewModel is Panel)
                {
                    _stringBuilder.AppendFormat(
                        "</gp:CollapsePanel>\n");
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
        /// Создает контрол в режиме Edit
        /// </summary>
        /// <param name="modalViewModel">Обьект поле</param>
        private void WriteEdit(WebPageBaseViewModel modalViewModel)
        {
            _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
            string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdEdit);
            switch (modalViewModel.Type)
            {
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
        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
