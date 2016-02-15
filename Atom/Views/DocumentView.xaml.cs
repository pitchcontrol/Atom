using System.Windows;

namespace Atom.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentView.xaml
    /// </summary>
    public partial class DocumentView : Window
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void Wizard_OnCancelled(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Wizard_OnFinished(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
