using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Atom.ViewModels;

namespace Atom.Services
{
    public class ScriptConstructorHelper
    {
        StringBuilder _stringBuilder = new StringBuilder();

        public ScriptConstructorHelper()
        {
            _stringBuilder.AppendFormat("DECLARE @id int;\n");
            Visability = 3;
        }

        public void Constructor(IEnumerable<WebPageBaseViewModel> collection, bool isEdit, int menupage,
            int role)
        {
            Constructor(collection, isEdit, menupage, new int[] { role });
        }

        public int Visability { get; set; }

        /// <summary>
        /// Построить скрип авторизации
        /// </summary>
        /// <param name="collection">Колекция контролов</param>
        /// <param name="isEdit"></param>
        /// <param name="menupage">Ид меню</param>
        /// <param name="roles">Ид роли</param>
        public void Constructor(IEnumerable<WebPageBaseViewModel> collection, bool isEdit, int menupage, IEnumerable<int> roles)
        {
            foreach (WebPageBaseViewModel model in collection)
            {
                string description = string.Format("ru-RU:{0};en-EN:{1};", model.RuDescription, model.EnDescription);
                string table = "null";
                if (model is ModalViewModel)
                {
                    ModalViewModel m = model as ModalViewModel;
                    if (!string.IsNullOrEmpty(m.TableName))
                        table = "'" + m.TableName + "'";
                }
                _stringBuilder.AppendFormat("--{0}\n", model.FieldInDb);
                _stringBuilder.AppendFormat(
                        "INSERT INTO [ut_MenuField] (idpage,fld, idparent, fldbd, tabbd, isNotEdited, nam) VALUES ({0}, '{1}', null, '{2}', {3}, 0, '{4}');\n",
                        menupage,
                        isEdit ? model.ControlIdEdit : model.ControlIdView,
                        model.FieldInDb ?? "null",
                        table,
                        description);

                _stringBuilder.AppendFormat("set @id  = scope_identity();\n");
                _stringBuilder.AppendFormat("insert into [ut_RoleField] (idrole, idfld,visability)\nvalues\n");

                string rs = roles.Aggregate("", (s, a) =>
                {
                    s += string.Format("({0},@id,{1}),\n", a, Visability);
                    return s;
                }, (result) => result.TrimEnd(new char[] { ',', '\n' }));
                _stringBuilder.AppendFormat(rs + "\n");
                Constructor(model.Children, isEdit, menupage, roles);
            }
        }
        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
