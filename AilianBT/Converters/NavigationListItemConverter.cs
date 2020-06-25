using AilianBT.Models;
using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class NavigationListItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (NavigationListItem)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (NavigationListItem)value;
        }
    }
}
