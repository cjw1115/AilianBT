using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace PullTofresh
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page,INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.scrollviewer.ViewChanged += Scrollviewer_ViewChanged;
            this.scrollviewer.Loaded += Scrollviewer_Loaded;
            this.listview.Items.Add(1);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool _isActive = false;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }


        private void Scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            scrollviewer.ChangeView(null, 120, null);
        }

        private void Scrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
        }
    }
}
