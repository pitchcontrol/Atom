using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using AvalonWizard;

namespace Atom.Behavior
{
    public class CurrentIndexBehavior: Behavior<Wizard>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CurrentPageChanged += AssociatedObject_CurrentPageChanged;
        }

        public static readonly DependencyProperty CurrentPageIndexProperty = DependencyProperty.Register(
            "CurrentPageIndex", typeof (int), typeof (CurrentIndexBehavior), new PropertyMetadata(default(int)));

        public int CurrentPageIndex
        {
            get { return (int) GetValue(CurrentPageIndexProperty); }
            set { SetValue(CurrentPageIndexProperty, value); }
        }

        private void AssociatedObject_CurrentPageChanged(object sender, CurrentPageChangedEventArgs e)
        {
            var item = sender as Wizard;
            if (item != null)
            {
                CurrentPageIndex = item.CurrentPageIndex;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.CurrentPageChanged -= AssociatedObject_CurrentPageChanged;
            }
        }
    }
}
