using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class TimeSpanStrConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan duration;
            if (value is double seconds)
            {
                duration = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                duration = (TimeSpan)value;
            }
            return duration.ToString(@"mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
