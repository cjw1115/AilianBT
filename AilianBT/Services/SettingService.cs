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
            view.TitleBar.ButtonBackgroundColor = targetColor;
            view.TitleBar.BackgroundColor = targetColor;
            view.TitleBar.InactiveBackgroundColor = targetColor;
            view.TitleBar.ButtonInactiveBackgroundColor = targetColor;

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
    }
}
