using BtDownload.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingView : Page
    {
        public SettingView()
        {
            this.InitializeComponent();
            this.Loaded += SettingView_Loaded;
        }

        private async void SettingView_Loaded(object sender, RoutedEventArgs e)
        {
            var folder =await FileService.GetDownloadFolder();
            tbDownload.Content = folder.Path;
        }

        private async void  DownloadLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".ailianbt");
            var folder=await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                ((Button)sender).Content = folder.Path;
                FileService.SetDownloadFolder(folder);
            }
            
        }
    }
}
