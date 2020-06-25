using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var toady = DateTime.Now.DayOfWeek;

            int dayIndex = (int)value;
            if ((int)toady == dayIndex)
                return "今天";
            switch (dayIndex)
            {
                case 0: return "星期日";
                case 1: return "星期一";
                case 2: return "星期二";
                case 3: return "星期三";
                case 4: return "星期四";
                case 5: return "星期五";
                case 6: return "星期六";
                case 7: return "上季完";
                default: return dayIndex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
