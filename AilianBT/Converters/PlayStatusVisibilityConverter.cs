using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class PlayStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is string title && string.IsNullOrWhiteSpace(title))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
