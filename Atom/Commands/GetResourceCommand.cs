using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Atom.Commands
{
    /// <summary>
    /// Команда формирует тэги для ресурсов
    /// </summary>
    public class GetResourceCommand : ICommand
    {
        private readonly ShellViewModel _model;

        public GetResourceCommand(ShellViewModel model)
        {
            _model = model;
            _model.Properties.CollectionChanged += (s, e) =>
            {
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, e);
            };
        }

        public bool CanExecute(object parameter)
        {
            return _model.Properties.Count != 0;
        }

        public void Execute(object parameter)
        {
            StringBuilder sbr = new StringBuilder();
            StringBuilder sbe = new StringBuilder();
            bool isEdit = parameter.ToString() == "True";
            WebPageBaseViewModel rootModel = _model.Properties.FirstOrDefault();
            Constructor(sbr, sbe, rootModel.Children, isEdit);
            //_model.ResuorseTextRu = sbr.ToString();
            //_model.ResuorseTextEn = sbe.ToString();
        }

        private void Constructor(StringBuilder sbr, StringBuilder sbe, IEnumerable<WebPageBaseViewModel> collection, bool isEdit)
        {
            foreach (WebPageBaseViewModel modalViewModel in collection)
            {
                sbr.AppendFormat("<data name=\"{0}\" xml:space=\"preserve\">\n", isEdit ? modalViewModel.ControlIdEdit : modalViewModel.ControlIdView);
                sbr.AppendFormat("<value>{0}</value>\n</data>\n", modalViewModel.RuDescription);

                sbe.AppendFormat("<data name=\"{0}\" xml:space=\"preserve\">\n", isEdit ? modalViewModel.ControlIdEdit : modalViewModel.ControlIdView);
                sbe.AppendFormat("<value>{0}</value>\n</data>\n", modalViewModel.EnDescription);

                Constructor(sbr, sbe, modalViewModel.Children, isEdit);
            }
           
            //ResXResourceWriter resXResource = new ResXResourceWriter();
        }

        public event EventHandler CanExecuteChanged;
    }
}
