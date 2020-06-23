using AilianBT.Constant;
using AilianBT.Helpers;
using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace AilianBT.Views
{
    public sealed partial class SettingView : Page,INotifyPropertyChanged
    {
        public string Version =>  $"{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}";
        private StorageService _storageService = SimpleIoc.Default.GetInstance<StorageService>();
        private DownloadService _downloadService = SimpleIoc.Default.GetInstance<DownloadService>();

        public SettingView()
        {
            this.InitializeComponent();
            this.Loaded += SettingView_Loaded;
        }

        private async void SettingView_Loaded(object sender, RoutedEventArgs e)
        {
            this.imgbackground.Source = await AssertsHelper.GetRandomBackgroundImage();

            var folder =await _downloadService.GetDownloadFolder();
            if(folder!=null)
            {
                tbDownload.Content = folder.Path;
            }

            var taostswitch= _storageService.GetLocalSetting<bool>("toastswitch");
            this.toast_switch.IsOn = taostswitch;

            var livingmode = _storageService.GetLocalSetting<bool?>(Definition.KISSSUB_LIVING_MODE);
            if(livingmode==null)
            {
                this.switchLivingMode.IsOn = true;
                _storageService.SetLocalSetting<bool?>(Definition.KISSSUB_LIVING_MODE, true);
            }
            else
            {
                this.switchLivingMode.IsOn = livingmode.Value;
            }

            ThemeColors.Clear();
            var colors = Services.SettingService.GetAllColor();
            foreach (var item in colors)
            {
                ThemeColors.Add(item);
            }

            var theme = _storageService.GetLocalSetting<ThemeColorModel>(nameof(ThemeColorModel));
            if(theme==null)
            {
                theme = ThemeColors.First();
                _storageService.SetLocalSetting(nameof(ThemeColorModel), theme);
            }
            SelectedTheme= ThemeColors.Where(m=>m.ThemeColor==theme.ThemeColor).First();
        }

        private async void  DownloadLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".ailianbt");
            var folder=await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                ((Button)sender).Content = folder.Path;
                _downloadService.SetDownloadFolder(folder);
            }
        }
        
        private void toast_switch_Toggled(object sender, RoutedEventArgs e)
        {
            _storageService.SetLocalSetting<bool>("toastswitch", toast_switch.IsOn);
        }

        #region 用户主题设置
        public ObservableCollection<Models.ThemeColorModel> ThemeColors { get; set; } = new ObservableCollection<Models.ThemeColorModel>();

        private Models.ThemeColorModel _selectedTheme;
        public Models.ThemeColorModel SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if(value!=_selectedTheme)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                }
            } 
        }
        public async void ThemeItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Models.ThemeColorModel;
            Services.SettingService.SetThemeColor(item.ThemeColor);
            SelectedTheme = item;

            _storageService.SetLocalSetting(nameof(ThemeColorModel), item);
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName=null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        private void switchLivingMode_Toggled(object sender, RoutedEventArgs e)
        {
            _storageService.SetLocalSetting<bool?>(Constant.Definition.KISSSUB_LIVING_MODE, switchLivingMode.IsOn);
        }
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
