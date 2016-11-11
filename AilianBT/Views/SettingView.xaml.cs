using BtDownload.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI;
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
            this.imgbackground.Source = await FileService.GetBackgroundImage();

            var folder =await FileService.GetDownloadFolder();
            tbDownload.Content = folder.Path;
            var taostswitch=FileService.GetLocalSetting<bool>("toastswitch");
            this.toast_switch.IsOn = taostswitch;

            ThemeColors.Clear();
            var colors = Services.SettingService.GetAllColor();
            foreach (var item in colors)
            {
                ThemeColors.Add(item);
            }
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
        
        private void toast_switch_Toggled(object sender, RoutedEventArgs e)
        {
            FileService.SetLocalSetting<bool>("toastswitch", toast_switch.IsOn);
        }

        #region 用户主题设置
        public ObservableCollection<Models.ThemeColorModel> ThemeColors { get; set; } = new ObservableCollection<Models.ThemeColorModel>();
        public async void ThemeItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Models.ThemeColorModel;
            Services.SettingService.SetThemeColor(item.ThemeColor);

            //存储颜色
            AilianBT.DAL.LocalSetting setting = new DAL.LocalSetting();
            await setting.SetLocalInfo<Models.ThemeColorModel>(typeof(Models.ThemeColorModel).Name, item);
        }
        #endregion

    }
    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
