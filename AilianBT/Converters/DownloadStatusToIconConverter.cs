using AilianBT.Models;
using System;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    public class DownloadStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DownloadStatus status = (DownloadStatus)value;
            switch (status)
            {
                case DownloadStatus.NoStart:
                    return "\xE896";
                case DownloadStatus.Pause:
                    return "\xE896";
                case DownloadStatus.Run:
                    return "\xE769";
                case DownloadStatus.Complate:
                case DownloadStatus.Other:
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
