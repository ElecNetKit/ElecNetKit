using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ElecNetKitExplorer.Converters
{
    /// <summary>
    /// Helper class to convert from bool to a Visibility value.
    /// Used to Bind CheckBox.IsChecked property to whether
    /// a control is visible.
    /// The word 'invert' can be supplied as a parameter, upon which
    /// the value will be flipped.
    /// </summary>
    class Bool2VisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
            {
                bool bValue = (bool)value;
                if (parameter != null && 
                    parameter.GetType() == typeof(string) &&
                    ((string)parameter).ToLower() == "invert")
                {
                    bValue = !bValue;
                }
                if (bValue)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
