using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class DataSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ulong bytes = (ulong)value;
            double temp;
            string final=(temp = bytes / 1024d) < 1024d 
                        ? Math.Round(temp,2) + "KB" 
                        : (temp /= 1024d) < 1024d
                            ? Math.Round(temp, 2) + "MB" 
                            : (temp /= 1024d) < 1024d 
                                ? Math.Round(temp, 2) + "GB" 
                                : Math.Round((temp /1024d),2) + "TB";
            return final;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
