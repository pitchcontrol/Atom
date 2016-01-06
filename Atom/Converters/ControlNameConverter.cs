using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Atom.Converters
{
    public class ControlNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string) values[3] == "RootPanel" /*DependencyProperty.UnsetValue*/)
            {
                return values[0];
            }
            if ((string)values[3] == "panel" /*DependencyProperty.UnsetValue*/)
            {
                return string.Format("[{0}] [{2}]", values);
            }
            return string.Format("[{0}] [{1}].[{2}]", values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
