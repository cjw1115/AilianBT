using BtDownload.Services;
using BtDownload.VIewModels;
using BtDownload.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BtDownload.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownloadMainView : Page
    {
        
        public DownloadMainView()
        {
            this.InitializeComponent();
            
        }

        private async void  piovt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            DownloadedVM downloadedVM = SimpleIoc.GetInstance<DownloadedVM>();
            var PivotItem = (PivotItem)this.piovt.SelectedItem;
            if (PivotItem != null)
            {
                var page = PivotItem.Content;
                if (page is DownloadedView)
                {
                    var list = DownloadedView.GetDownloadedInfo();
                    downloadedVM.DownloadedInfoList.Clear();
                    foreach (var item in list)
                    {   
                        downloadedVM.DownloadedInfoList.Add(item);
                    }
                }
            }
            
        }
    }
}
