﻿using AilianBT.Common.Models;
using AilianBT.Models;
using AilianBT.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MusicView : Page
    {
        public MusicVM MusicVM { get; set; }
        public MusicView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            MusicVM = new MusicVM(this.Dispatcher);
            this.Loaded += MusicView_Loaded;
        }

        private void MusicView_Loaded(object sender, RoutedEventArgs e)
        {
            MusicVM.Load();
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var grid=sender as FrameworkElement;
            //ListViewItemPresenter presenter = (ListViewItemPresenter)VisualTreeHelper.GetParent(grid);
            //ListViewItem item = (ListViewItem)VisualTreeHelper.GetParent(presenter);
            MusicVM.ItemClick(grid.DataContext as MusicModel);
        }
    }
    public class BoolToVisibility : IValueConverter
    {
        public bool Inverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool status = (bool)value;
            if(Inverse)
            {
                status = !status;
            }
            if (status == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayStatusVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string title = (string)value;
            if (string.IsNullOrWhiteSpace(title))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
