using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{

    public class BoolToVisibityConverter : IValueConverter
    {
        public bool Inverse { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (Inverse ? !(bool)value : (bool)value) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
