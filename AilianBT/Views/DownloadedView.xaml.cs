using AilianBT.Services;
using BtDownload.Models;
using BtDownload.Services;
using BtDownload.VIewModels;
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
    public sealed partial class DownloadedView : Page
    {
        public DownloadedVM DownloadedVM { get; set; }
        public DownloadedView()
        {
            
            this.InitializeComponent();
            //this.Loaded += DownloadedView_Loaded;
            DownloadedVM = SimpleIoc.GetInstance<DownloadedVM>();

            var dbservice = BtDownload.Services.SimpleIoc.GetInstance<DbService>();
            var list = dbservice.DownloadDbContext.DownloadedInfos;

            DownloadedVM.DownloadedInfoList.Clear();
            foreach (var item in list)
            {
                DownloadedVM.DownloadedInfoList.Add(item);
            }
        }

        

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            List<DownloadedInfo> removes = new List<DownloadedInfo>();
            var list=this.downList.SelectedItems;
            foreach (var item in list)
            {
                removes.Add((DownloadedInfo)item);
            }
            foreach (var item in removes)
            {
                DownloadedVM.DownloadedInfoList.Remove(item);
            }

            var dbservice = BtDownload.Services.SimpleIoc.GetInstance<DbService>();
            dbservice.DownloadDbContext.DownloadedInfos.RemoveRange(removes);
            dbservice.DownloadDbContext.SaveChanges();
            
            SetDefaultSelectStatus();
        }
        private async void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            List<DownloadedInfo> removes = new List<DownloadedInfo>();
            var list = this.downList.SelectedItems;
            foreach (var item in list)
            {
                removes.Add((DownloadedInfo)item);
            }
            foreach (var item in removes)
            {
                DownloadedVM.DownloadedInfoList.Remove(item);
            }

            var dbservice = BtDownload.Services.SimpleIoc.GetInstance<DbService>();
            dbservice.DownloadDbContext.DownloadedInfos.RemoveRange(removes);
            dbservice.DownloadDbContext.SaveChanges();
            

            try
            {
                foreach (var item in removes)
                {
                    //var file = await StorageFile.GetFileFromPathAsync(item.FilePath);
                    var file = await FileService.GetFile(item.FilePath);
                    await FileService.DeleteFile(file);
                }
            }
            catch (FileNotFoundException notfound)
            {
                AilianBT.App.ShowNotification("文件不存在：" + notfound.Message);
            }
            SetDefaultSelectStatus();
        }


        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (!DownloadedVM.IsSelecting)
            {
                DownloadedVM.IsSelecting = true;
                this.downList.IsMultiSelectCheckBoxEnabled = true;
                this.downList.SelectionMode = ListViewSelectionMode.Multiple;
                btnSelectAll.Label = "完成";
            }
            else
            {
                DownloadedVM.IsSelecting = false;
                this.downList.IsMultiSelectCheckBoxEnabled = false;
                this.downList.SelectionMode = ListViewSelectionMode.Single;
                btnSelectAll.Label = "选择";
            }
        }

        private void downList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = this.downList.SelectedItems;
            if (list == null)
            {
                this.DownloadedVM.IsEnableRemove = false;
                
                return;
            }
            if (list.Count > 0)
                this.DownloadedVM.IsEnableRemove = true;
            else
                this.DownloadedVM.IsEnableRemove = false;

            
        }

        private void SetDefaultSelectStatus()
        {
            DownloadedVM.IsSelecting = false;
            this.downList.IsMultiSelectCheckBoxEnabled = false;
            this.downList.SelectionMode = ListViewSelectionMode.Single;
            btnSelectAll.Label = "选择";
        }


       

        
    }
}
