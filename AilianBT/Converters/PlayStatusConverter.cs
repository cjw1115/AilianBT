using AilianBT.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AilianBT.Converters
{
    class PlayStatusConverter:IValueConverter
    {
        public PlayerStatus StatusToVisible { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility? visibility = null;
            var currentStatus = (PlayerStatus)value;
            switch (StatusToVisible)
            {
                case PlayerStatus.Playing:
                    visibility = currentStatus == PlayerStatus.Playing ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    visibility = currentStatus != PlayerStatus.Playing ? Visibility.Visible : Visibility.Collapsed;
                    break;
                default:
                    break;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
