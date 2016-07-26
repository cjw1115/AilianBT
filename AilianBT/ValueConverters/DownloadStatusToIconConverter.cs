using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using BtDownload.Models;
namespace BtDownload.ValueConverters
{
    public class DownloadStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DownloadStatus status = (DownloadStatus)value;
            switch (status)
            {
                case DownloadStatus.NoStart:
                    return "开始";
                case DownloadStatus.Pause:
                    return "开始";
                case DownloadStatus.Run:
                    return "暂停";
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
