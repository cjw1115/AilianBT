using AilianBT.Constant;
using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AilianBT.ViewModels
{
    public class SettingViewModel:ViewModelBase
    {
        private StorageService _storageService = SimpleIoc.Default.GetInstance<StorageService>();
        private DownloadService _downloadService = SimpleIoc.Default.GetInstance<DownloadService>();
        private SettingService _settingService = SimpleIoc.Default.GetInstance<SettingService>();
        private LogService _logService = SimpleIoc.Default.GetInstance<LogService>();

        public async void Load()
        {
            _logService.Debug("Try to load all of settings");

            var folder = await _downloadService.GetDownloadFolder();
            if (folder != null)
            {
                _logService.Debug($"Default download folder: {folder.Path}");
                DownloadFolder = folder.Path;
            }

            IsToastOn = _storageService.GetLocalSetting<bool>(Definition.SETTING_TOAST);
            _logService.Debug($"Toast notification is turned " + (_isToastOn ? "on" : "off"));

            IsLiveModeOn = _settingService.GetLiveMode().Value;

            _loadAutoDownloadMagnetSettings();

            _loadThemeSettings();
        }

        #region Version info
        public string Version => $"{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}";
        #endregion

        #region Toast notification
        private bool _isToastOn;
        public bool IsToastOn
        {
            get => _isToastOn;
            set
            { 
                if (Set(ref _isToastOn, value))
                {
                    _logService.Debug($"Try to set toast notification to " + (value ? "on" : "off"));
                    _storageService.SetLocalSetting<bool>(Definition.SETTING_TOAST, value);
                }
            }
        }
        #endregion

        #region Living mode
        public bool _isLiveModeOn;
        public bool IsLiveModeOn
        {
            get => _isLiveModeOn;
            set 
            {
                if(Set(ref _isLiveModeOn, value))
                {
                    _settingService.SetLiveMode(value);
                }
            } 
        }
        #endregion

        #region Download dolder
        public string _downloadFolder;
        public string DownloadFolder
        {
            get => _downloadFolder;
            set => Set(ref _downloadFolder, value);
        }

        public async void DownloadLocationClicked(object sender, RoutedEventArgs e)
        {
            var folder = await _downloadService.PickDefaultDownloadFolder();
            if (folder != null)
            {
                DownloadFolder = folder.Path;
            }
        }
        #endregion

        #region 用户主题设置
        public ObservableCollection<ThemeColorModel> ThemeColors { get; set; } = new ObservableCollection<ThemeColorModel>();

        private ThemeColorModel _selectedTheme;
        public ThemeColorModel SelectedTheme
        {
            get => _selectedTheme;
            set => Set(ref _selectedTheme, value);
        }

        public void ThemeItemClicked(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ThemeColorModel;
            _settingService.SetThemeColor(item);
            SelectedTheme = item;

            _storageService.SetLocalSetting(nameof(ThemeColorModel), item);
        }

        private void _loadThemeSettings()
        {
            ThemeColors.Clear();
            var colors = _settingService.GetAllColor();
            foreach (var item in colors)
            {
                ThemeColors.Add(item);
            }

            var theme = _settingService.GetCachedTheme();
            if (theme == null)
            {
                var currentColor = _settingService.GetCurrentThemeBrush().Color;
                theme = ThemeColors.Single(m => m.ThemeColor == currentColor);
            }
            SelectedTheme = ThemeColors.Single(m => m.ThemeColor == theme.ThemeColor);
        }
        #endregion

        #region 磁链自动下载
        public bool _isAutoDownloadMagnet;
        public bool IsAutoDownloadMagnet
        {
            get => _isAutoDownloadMagnet;
            set
            {
                if (Set(ref _isAutoDownloadMagnet, value))
                {
                    _settingService.SetMagnetAutoDownloadStatus(value);
                }
            }
        }

        public string _autoDownloadMagnetProtocal;
        public string AutoDownloadMagnetProtocal
        {
            get => _autoDownloadMagnetProtocal;
            set => Set(ref _autoDownloadMagnetProtocal, value);
        }

        private void _loadAutoDownloadMagnetSettings()
        {
            IsAutoDownloadMagnet = _settingService.GetMagnetAutoDownloadStatus() ?? false;
            var protocal = _settingService.GetMagnetAutoDownloadProtocal() ?? string.Empty;
            if(string.IsNullOrEmpty(protocal))
            {
                protocal = Definition.SETTING_MAGNET_AUTO_DOWNLOAD_PROTOCAL_THUNDER;
            }
            AutoDownloadMagnetProtocal = protocal;
        }

        public void SetAutoDownloadMagnetSettings()
        {
            _settingService.SetMagnetAutoDownloadProtocal(AutoDownloadMagnetProtocal);
        }
        #endregion
    }
}
