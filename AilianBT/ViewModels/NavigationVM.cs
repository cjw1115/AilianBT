﻿using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BtDownload.Views;
namespace AilianBT.ViewModels
{
    public class NavigationVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        public List<NavigationListItem> NavigationList { get; set; } = new List<NavigationListItem>
        {
           new NavigationListItem { Title="首页", PageType=typeof(Views.MainView) },
           new NavigationListItem { Title="下载中心", PageType=typeof(DownloadMainView) }
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
            Models.NavigationListItem item = NavigationList[SelectedIndex];
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

        public void Setting_Click()
        {
            SelectedIndex = -1;
            MainTitle = "设置";
            FuncFrame.Navigate(typeof(Views.SettingView));
        }
    }
}
