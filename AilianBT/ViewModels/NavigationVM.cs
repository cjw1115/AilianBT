using AilianBT.Models;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
namespace AilianBT.ViewModels
{
    public class NavigationVM:ViewModelBase
    {
        private readonly string messageToken = "1";

        public List<NavigationListItem> NavigationList { get; set; } = new List<NavigationListItem>
        {
           new NavigationListItem { Title="首页", PageType=typeof(Views.MainView),Icon="\xE80F" },
           new NavigationListItem { Title="番组", PageType=typeof(Views.KeyView),Icon="\xE192"},
           new NavigationListItem { Title="音乐", PageType=typeof(Views.MusicView),Icon="\xE8D6"},
           new NavigationListItem { Title="下载", PageType=typeof(Views.DownloadMainView) ,Icon="\xE896"}
        };

        public List<NavigationListItem> NavigationFooterList { get; set; } = new List<NavigationListItem>
        {
           new NavigationListItem { Title="设置", Icon="\xE713"}
        };

        public static Frame FuncFrame { get; set; }
        public static Frame DetailFrame { get; set; }
        
        private string _mainTitle;
        public string MainTitle
        {
            get { return _mainTitle; }
            set { Set(ref _mainTitle, value); }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIndex < 0)
                return;
            NavigationListItem item = NavigationList[SelectedIndex];
            if (item != null && item.PageType != null)
            {
                MainTitle = item.Title;
                FuncFrame.Navigate(item.PageType);
            }
            else
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("这只是个饼，而且还没画完O(∩_∩)O", messageToken);
            }
        }

        public void SettingClicked()
        {
            MainTitle = "设置";
            FuncFrame.Navigate(typeof(Views.SettingView));
        }
    }
}
