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
using Windows.Networking;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using BtDownload.Services;
using BtDownload.VIewModels;
using BtDownload.Models;
using Windows.Storage.AccessCache;
using AilianBT.Services;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace BtDownload.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownloadingView : Page
    {
        private DownloadService _downloadService;
        public DownloadingVM DownloadingVM { get; set; }
        public DownloadedVM DownloadedVM { get; set; }
        public DownloadingView()
        {
            DownloadingVM = SimpleIoc.GetInstance<DownloadingVM>();
            DownloadedVM = SimpleIoc.GetInstance<DownloadedVM>();
            _downloadService = new DownloadService();

            this.InitializeComponent();
            this.Loaded += DownloadingView_Loaded;

            
        }

        private async void DownloadingView_Loaded(object sender, RoutedEventArgs e)
        {
            List<Task> downloadtasks = new List<Task>();
            var downloadInfos = await _downloadService.GetAllDownloadInfoAsync(DownloadProgress);
            DownloadingVM.DownloadOperations.Clear();
            foreach (var item in downloadInfos)
            {
                DownloadingVM.DownloadOperations.Add(item);

                var task=item.AttachAsync();
                task.ContinueWith(async _=> await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    ()=> FinishedDownload(item)));
                downloadtasks.Add(task);
            }
            try
            {
                await Task.WhenAll(downloadtasks);
            }
            catch (TaskCanceledException cancell)
            {

            }
        }
        private StorageFolder defaultFolder;
        private async Task SetStorageFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".ailianbt");
            defaultFolder = await picker.PickSingleFolderAsync();

            FileService.SetDownloadFolder(defaultFolder);
        }

        public async void FinishedDownload(DownloadInfo downloadInfo)
        {
            DownloadingVM.DownloadOperations.Remove(downloadInfo);
            var re=await DownloadService.FinishedDownload(downloadInfo);
            
            DownloadedVM.DownloadedInfoList.Add(re);

            //存储到数据库中
            DownloadService.StoreDownloadedInfo(re);

            NotificationService.ShowDownloadFinishedToast(downloadInfo.FileName);
        }

        public async Task StarDownload(string filename,Uri uri)
        {
            //创建文件
            //td:此处添加文件存在检测逻辑
            //
            var file = await FileService.CreaterFile(defaultFolder, filename);

            var downloadinfo = await _downloadService.CreateDownload(uri, file, DownloadProgress);
            DownloadingVM.DownloadOperations.Add(downloadinfo);

            try
            {
                await downloadinfo.Start();

                //下载完成动作
                DownloadingVM.DownloadOperations.Remove(downloadinfo);
                FinishedDownload(downloadinfo);
            }
            catch (TaskCanceledException exception)
            {

            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //await SetStorageFolder();

            defaultFolder = await FileService.GetDownloadFolder();
            if (defaultFolder == null)
            {
                AilianBT.App.ShowNotification("需要选择默认下载地址");
                return;
            }
                
            try
            {
                var uri = DownloadService.GetDownloadUri(this.tbUri.Text);
                var filename = DownloadService.GetDownloadFileName(this.tbUri.Text);
                await StarDownload(filename, uri);
            }
            catch(System.UriFormatException uriFormartException)
            {
                AilianBT.App.ShowNotification("下载地址不正确：" + uriFormartException.Message);
            }
            catch(ArgumentException argumentException)
            {
                AilianBT.App.ShowNotification("下载参数异常：" + argumentException.Message);
            }
            catch(Exception exception)
            {
                AilianBT.App.ShowNotification("下载超时");
            }
            

        }
        public void DownloadProgress(DownloadOperation operation)
        {

            var re = DownloadingVM.DownloadOperations.Where(m => m.DownloadOperation == operation).FirstOrDefault();
            if (re != null)
            {

                re.TotalToReceive = operation.Progress.TotalBytesToReceive;
                re.ReceivedBytes = operation.Progress.BytesReceived;
                if (re.TotalToReceive != 0)
                {
                    re.FinishedPercent = Math.Round((double)(re.ReceivedBytes * 100) / re.TotalToReceive, 2);
                }

            }
        }

        

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var id = (int)btn.Tag;
            var info=DownloadingVM.DownloadOperations.Where(m => m.ID == id).FirstOrDefault();
            if (info.DownloadStatus == DownloadStatus.Run)
            {
                info.Pause();
                //btn.Content = "\xE768;";//开始按钮

            }
            else if (info.DownloadStatus == DownloadStatus.Pause)
            {
                info.Resume();
                //btn.Content = "\xE769;";//暂停按钮
            }
            var sel = this.downList.SelectedItem as DownloadInfo;
            if (sel != null && sel.ID == id)
            {
                if (info.DownloadStatus == DownloadStatus.Run)
                {
                    DownloadingVM.IsEnablePause = true;
                    DownloadingVM.IsEnableStart = false;

                }
                else if (info.DownloadStatus == DownloadStatus.Pause)
                {
                    DownloadingVM.IsEnablePause = false;
                    DownloadingVM.IsEnableStart = true;
                }
            }

        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            var list = this.downList.SelectedItems;
            if (list == null)
                return;
            
            var pausedList = list.Where(m =>((DownloadInfo)m).DownloadStatus== DownloadStatus.Pause).ToList();
            foreach (var item in pausedList)
            {
                ((DownloadInfo)item).Resume();
            }

            SetDefaultSelectStatus();
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {

            var list = this.downList.SelectedItems;
            if (list == null)
                return;
            
            var runningList = list.Where(m => ((DownloadInfo)m).DownloadStatus==DownloadStatus.Run).ToList();
            foreach (var item in runningList)
            {
                ((DownloadInfo)item).Pause();
            }
            SetDefaultSelectStatus();

        }
        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            List<DownloadInfo> removes = new List<DownloadInfo>();
            foreach (var item in this.downList.SelectedItems)
            {
                DownloadInfo info = item as DownloadInfo;
                info.Cts.Cancel();
                info.Cts.Dispose();
                removes.Add(info);

            }
            foreach (var item in removes)
            {
                this.DownloadingVM.DownloadOperations.Remove(item);
            }
            try
            {
                foreach (var item in removes)
                {
                    await FileService.DeleteFile((StorageFile)item.DownloadOperation.ResultFile);
                }
            }
            catch(FileNotFoundException notfound)
            {
                AilianBT.App.ShowNotification("文件不存在："+ notfound.Message);
            }
            
            //页面恢复未选择状态
            SetDefaultSelectStatus();
        }

        private void SetDefaultSelectStatus()
        {
            downList_SelectionChanged(null, null);

            DownloadingVM.IsSelecting = false;
            this.downList.IsMultiSelectCheckBoxEnabled = false;
            this.downList.SelectionMode = ListViewSelectionMode.Single;
            btnSelectAll.Label = "选择";

        }
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            if (!DownloadingVM.IsSelecting)
            {
                DownloadingVM.IsSelecting = true;
                this.downList.IsMultiSelectCheckBoxEnabled = true;
                this.downList.SelectionMode = ListViewSelectionMode.Multiple;
                btnSelectAll.Label = "完成";
            }
            else
            {
                DownloadingVM.IsSelecting = false;
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
                this.DownloadingVM.IsEnableRemove = false;
                this.DownloadingVM.IsEnablePause = false;
                this.DownloadingVM.IsEnableStart = false;
                return;
            }   
            if (list.Count > 0)
                this.DownloadingVM.IsEnableRemove = true;
            else
                this.DownloadingVM.IsEnableRemove = false;

            if (list.FirstOrDefault(m => ((DownloadInfo)m).DownloadStatus== DownloadStatus.Pause) != null)
                this.DownloadingVM.IsEnableStart = true;
            else
                this.DownloadingVM.IsEnableStart = false;

            if (list.FirstOrDefault(m => ((DownloadInfo)m).DownloadStatus== DownloadStatus.Run) != null)
                this.DownloadingVM.IsEnablePause = true;
            else
                this.DownloadingVM.IsEnablePause = false;
        }
    }

    
}
