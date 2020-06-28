using AilianBT.Constant;
using AilianBT.Models;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AilianBT.Services
{
    public class SettingService
    {
        private StorageService _storageService;
        private LogService _logService;

        public SettingService(StorageService storageService, LogService logService)
        {
            _storageService = storageService;
            _logService = logService;
        }

        public List<ThemeColorModel> GetAllColor()
        {
            var list = new List<ThemeColorModel>();
            var colors = Application.Current.Resources["ThemeColors"] as ResourceDictionary;
            foreach (var item in colors)
            {
                var model = new ThemeColorModel();
                model.Name = (string)item.Key;
                model.ThemeColor = (Color)item.Value;
                list.Add(model);
            }
            return list;
        }

        public ThemeColorModel GetCachedTheme()
        {
            var model = _storageService.GetLocalSetting<ThemeColorModel>(nameof(ThemeColorModel));
            if (model == null)
            {
                _logService.Debug($"Didn't find the cached theme color");
            }
            else
            {
                _logService.Debug($"Cached theme color is {model.ThemeColor.ToString()}");
            }
            return model;
        }

        public void SetThemeColor(ThemeColorModel theme)
        {
            var currentBrush = GetCurrentThemeBrush();
            Color targetColor = theme == null ? currentBrush.Color : theme.ThemeColor;

            _logService.Debug($"Try to set theme color to {targetColor.ToString()}");

            currentBrush.Color = targetColor;
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            view.TitleBar.BackgroundColor = Colors.Transparent;
            view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            view.TitleBar.ButtonForegroundColor = Colors.Black;
            
            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = targetColor;
            }
        }

        public SolidColorBrush GetCurrentThemeBrush()
        {
            var brush =  Application.Current.Resources["AilianBtMainColor"] as SolidColorBrush;
            _logService.Debug($"Current theme color is {brush.Color.ToString()}");
            return brush;
        }

        public void AdjustTitleBar()
        {
            _logService.Debug($"Try to extend content view to title bar");
            Windows.ApplicationModel.Core.CoreApplication.MainView.TitleBar.ExtendViewIntoTitleBar = true;
        }

        public bool? GetLiveMode()
        {
            var livingMode = _storageService.GetLocalSetting<bool?>(Definition.KISSSUB_LIVING_MODE);
            if (livingMode == null)
            {
                _logService.Debug($"Living mode is not set yet, now try to set it to on by default");
            }
            else
            {
                _logService.Debug($"Living mode is set to " + (livingMode.Value ? "on" : "off"));
            }
            return livingMode;
        }

        public void SetLiveMode(bool mode)
        {
            _logService.Debug($"Try to set living mode to " + (mode ? "on" : "off"));
            _storageService.SetLocalSetting<bool?>(Definition.KISSSUB_LIVING_MODE, mode);
        }

        public bool? GetMagnetAutoDownloadStatus()
        {
            var status = _storageService.GetLocalSetting<bool?>(Definition.SETTING_MAGNET_AUTO_DOWNLOAD_ENABLE);
            if (status == null || status.Value == false)
            {
                _logService.Debug($"Magnet auto download with third patry tool is not enabled");
            }
            else
            {
                _logService.Debug($"Magnet auto download with third patry tool is enabled");
            }
            return status;
        }

        public void SetMagnetAutoDownloadStatus(bool mode)
        {
            _logService.Debug($"Try to set magnet auto download to " + (mode ? "on" : "off"));
            _storageService.SetLocalSetting<bool?>(Definition.SETTING_MAGNET_AUTO_DOWNLOAD_ENABLE, mode);
        }

        public string GetMagnetAutoDownloadProtocal()
        {
            var protocal = _storageService.GetLocalSetting<string>(Definition.SETTING_MAGNET_AUTO_DOWNLOAD_PROTOCAL);
            if (string.IsNullOrEmpty(protocal))
            {
                _logService.Debug($"Magnet auto download protocal is not set");
            }
            else
            {
                _logService.Debug($"Magnet auto download protocal is set to {protocal}");
            }
            return protocal;
        }

        public void SetMagnetAutoDownloadProtocal(string protocal)
        {
            _logService.Debug($"Try to set magnet auto download protocal to {protocal}");
            _storageService.SetLocalSetting<string>(Definition.SETTING_MAGNET_AUTO_DOWNLOAD_PROTOCAL, protocal);
        }
    }
}
