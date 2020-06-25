using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class NullToVisibityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is string str && string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
