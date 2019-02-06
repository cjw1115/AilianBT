using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AilianBT.ValueConverters
{
    public class PadleftConverter : IValueConverter
    {
        public int PadCount { get; set; } = 0;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var id = (int)value;
            return (id + 1).ToString().PadLeft(PadCount,'0');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
