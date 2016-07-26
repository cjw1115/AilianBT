using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;


namespace BtDownload.ValueConverters
{
    public class DataSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double temp = 0;
            ulong bytes = (ulong)value;
            string final=(temp = bytes / 1024d) < 1024d ? Math.Round(temp,2) + "KB" : (temp /= 1024d) < 1024d ? Math.Round(temp, 2) + "MB" : (temp /= 1024d) < 1024d ? Math.Round(temp, 2) + "GB" : Math.Round((temp /1024d),2) + "TB";
            return final;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
