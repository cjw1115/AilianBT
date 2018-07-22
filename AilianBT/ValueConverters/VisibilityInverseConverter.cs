using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.ValueConverters
{
    public class VisibilityInverseConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility visibility = (Visibility)value;
            switch (visibility)
            {
                case Visibility.Visible:
                    return Visibility.Collapsed;
                case Visibility.Collapsed:
                    return Visibility.Visible;
                default:
                    break;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
