using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PylonRecon.UI.Utilities
{
    internal class BooleanInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolValue) return null;
            if (targetType == typeof(bool)) return !boolValue;
            if (targetType == typeof(Visibility)) return boolValue ? Visibility.Collapsed : Visibility.Visible;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
