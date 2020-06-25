using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class VisibilityInverseConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)(1 ^ (ushort)(Visibility)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
