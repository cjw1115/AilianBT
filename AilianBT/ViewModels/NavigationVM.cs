﻿using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
namespace AilianBT.ViewModels
{
    public class NavigationVM:ViewModelBase
    {
        public List<NavigationListItem> NavigationList { get; set; } = new List<NavigationListItem>
        {
           new NavigationListItem { Title="人类", PageType=typeof(Views.HumanTestView),Icon="\xEB68" },
           new NavigationListItem { Title="首页", PageType=typeof(Views.MainView),Icon="\xE80F" },
           new NavigationListItem { Title="番组", PageType=typeof(Views.KeyView),Icon="\xE192"},
           new NavigationListItem { Title="音乐", PageType=typeof(Views.MusicView),Icon="\xE8D6"},
           new NavigationListItem { Title="下载", PageType=typeof(Views.DownloadMainView) ,Icon="\xE896"}
        };

        public List<NavigationListItem> NavigationFooterList { get; set; } = new List<NavigationListItem>
        {
           new NavigationListItem { Title="设置", Icon="\xE713"}
        };

        public Frame FuncFrame { get; set; }
        public Frame DetailFrame { get; set; }
        
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
                ClearDetailFrame();
                FuncFrame.Navigate(item.PageType);
            }
        }

        public void SettingClicked()
        {
            ClearDetailFrame();
            FuncFrame.Navigate(typeof(Views.SettingView));
        }

        private void ClearDetailFrame()
        {
            while (DetailFrame.BackStackDepth >= 1)
            {
                if (DetailFrame.BackStackDepth == 1)
                {
                    DetailFrame.GoBack();
                    break;
                }
                DetailFrame.BackStack.RemoveAt(DetailFrame.BackStackDepth - 1);
            }
        }

        public void DetailFrameNavigate(Type viewType, object param = null)
        {
            ClearDetailFrame();
            DetailFrame.Navigate(viewType, param);
        }
    }
}
