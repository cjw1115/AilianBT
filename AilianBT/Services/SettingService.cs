using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AilianBT.Services
{
    public class SettingService
    {
        public static void SetThemeColor(Color color)
        {
            var themeColor = Application.Current.Resources["AilianBtMainColor"] as SolidColorBrush;
            if (themeColor != null && color != null)
            {
                themeColor.Color = color;
                var view = ApplicationView.GetForCurrentView();
                view.TitleBar.ButtonBackgroundColor = color;
                view.TitleBar.BackgroundColor = color;

                if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
                {
                    StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = color;
                }

            }
        }
        public static List<Models.ThemeColorModel> GetAllColor()
        {
            List<Models.ThemeColorModel> list = new List<Models.ThemeColorModel>();
            var colors = Application.Current.Resources["ThemeColors"] as ResourceDictionary;
            foreach (var item in colors)
            {
                Models.ThemeColorModel model = new Models.ThemeColorModel();
                model.Name = (string)item.Key;
                model.ThemeColor = (Color)item.Value;
                list.Add(model);
            }
            return list;
        }
    }
}
