using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class KeyView : Page
    {
        public ViewModels.KeyVM KeyVM { get; set; }
        public KeyView()
        {
            var locator = App.Current.Resources["Locator"] as ViewModels.ViewModelLocator;
            KeyVM = locator.KeyVM;
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += KeyView_Loaded;
        }

        private  void KeyView_Loaded(object sender, RoutedEventArgs e)
        {
            KeyVM.Loaded();
            
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            KeyVM.GridView_ItemClick(e.ClickedItem);
        }
    }

    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var toady = DateTime.Now.DayOfWeek;
            
            int dayIndex = (int)value;
            if ((int)toady == dayIndex)
                return "今天";
            switch (dayIndex)
            {
                case 0: return "星期日"; 
                case 1:return "星期一"; 
                case 2: return "星期二";
                case 3: return "星期三";
                case 4: return "星期四";
                case 5: return "星期五";
                case 6: return "星期六";
                case 7: return "上季完";
                default:return dayIndex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
