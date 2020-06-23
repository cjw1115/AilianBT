﻿using AilianBT.Services;
using BtDownload.Models;
using BtDownload.VIewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private DbService _dbService = SimpleIoc.Default.GetInstance<DbService>();
        private StorageService _storageService = SimpleIoc.Default.GetInstance<StorageService>();

        public DownloadedView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += DownloadedView_Loaded; ;
            DownloadedVM = SimpleIoc.Default.GetInstance<DownloadedVM>();

           
        }

        private void DownloadedView_Loaded(object sender, RoutedEventArgs e)
        {
            
            var list = _dbService.DownloadDbContext.DownloadedInfos;

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

            _dbService.DownloadDbContext.DownloadedInfos.RemoveRange(removes);
            _dbService.DownloadDbContext.SaveChanges();
            
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

            _dbService.DownloadDbContext.DownloadedInfos.RemoveRange(removes);
            _dbService.DownloadDbContext.SaveChanges();
            

            try
            {
                foreach (var item in removes)
                {
                    //var file = await StorageFile.GetFileFromPathAsync(item.FilePath);
                    var file = await _storageService.GetFile(item.FilePath);
                    await _storageService.DeleteFile(file);
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
