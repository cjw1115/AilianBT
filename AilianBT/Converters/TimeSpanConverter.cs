using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class TimeSpanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).ToString(@"mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
