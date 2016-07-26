using AilianBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AilianBT.ValueConverters
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
