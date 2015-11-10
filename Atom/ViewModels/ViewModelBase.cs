using System.ComponentModel;
using System.Runtime.CompilerServices;
using Atom.Annotations;

namespace Atom.ViewModels
{
    /// <summary>
    /// Базовая модель с валидацией и нотификацией
    /// </summary>
    public class ViewModelBase : ValidationBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}