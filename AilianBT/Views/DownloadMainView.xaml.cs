using BtDownload.Services;
using BtDownload.VIewModels;
using BtDownload.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BtDownload.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownloadMainView : Page
    {
        
        public DownloadMainView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }
    }
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (-1 >= (int)value)
            {
                return 0;
            }
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }
    }
}
