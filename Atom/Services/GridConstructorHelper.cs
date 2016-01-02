using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Atom.Services
{
    /// <summary>
    /// Построение грида
    /// </summary>
    public class GridConstructorHelper
    {
        /// <summary>
        /// Расположени ресурсов
        /// </summary>
        public string ResourceNamespace { get; set; }
        readonly StringBuilder _stringBuilder = new StringBuilder();
        public void Construct(IEnumerable<WebPageBaseViewModel> collection)
        {
            foreach (WebPageBaseViewModel modalViewModel in collection)
            {
                Write(modalViewModel);
                Construct(modalViewModel.Children);
            }
        }

        private void Write(WebPageBaseViewModel modalViewModel)
        {
            _stringBuilder.AppendFormat("<%--{0}--%>\n", modalViewModel.RuDescription);
            string caption = string.Format("<%$ Resources: {0}, {1} %>", ResourceNamespace, modalViewModel.ControlIdView);
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
            }
            _stringBuilder.AppendFormat("</ItemTemplate>\n");
            _stringBuilder.AppendFormat("</asp:TemplateField>\n");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<gp:GpGridView ID=\"gvObject\" Height=\"100%\" runat=\"server\" PageSize=\"10\" ShowWhenEmpty=\"False\" SkinID=\"gpGridView\" DataKeyNames=\"id\" PagerSettings-Visible=\"true\">\n");
            sb.AppendFormat("<Columns>\n");
            sb.Append(_stringBuilder);
            sb.AppendFormat("</Columns>\n");
            sb.AppendFormat("</gp:GpGridView>\n");
            return sb.ToString();
        }
    }
}
