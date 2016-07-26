using BtDownload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.Services
{
    public class DownloadService
    {
        private BackgroundDownloader _downloader;

        public DownloadService()
        {
            _downloader = new BackgroundDownloader(); 
        }
       
        public async Task<DownloadInfo> CreateDownload(Uri uri,IStorageFile file,Action<DownloadOperation> handler)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(handler);
            var operation = _downloader.CreateDownload(uri, file);


            DownloadInfo info = new DownloadInfo
            {
                Cts = cts,
                Progress = progress,
                DownloadOperation = operation,
                DownloadStatus = DownloadStatus.NoStart,
                FileName = file.Name
            };

            info.Thumb = await FileService.GetThumbBytes(file);

            info.ID = info.FileName.GetHashCode();

            return info;
        }
        
        public async Task<IList<DownloadInfo>> GetAllDownloadInfoAsync(Action<DownloadOperation> handler)
        {
            List<DownloadInfo> downloadList = new List<DownloadInfo>();
            var downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            if (null != downloads)
            {
                //List<Task> downloadtasks = new List<Task>();
                foreach (var item in downloads)
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    DownloadInfo info = new DownloadInfo()
                    {
                        Cts = cts,
                        DownloadOperation = item,
                        FileName = item.ResultFile.Name,
                        ReceivedBytes = item.Progress.BytesReceived,
                        TotalToReceive = item.Progress.TotalBytesToReceive
                    };
                    if (info.TotalToReceive != 0)
                        info.FinishedPercent = Math.Round((double)(info.ReceivedBytes * 100) / info.TotalToReceive, 2);

                    info.ID = info.FileName.GetHashCode();
                    info.Cts = cts;

                    info.Thumb = await FileService.GetThumbBytes(item.ResultFile);


                    Progress<DownloadOperation> progress = new Progress<DownloadOperation>(handler);
                    info.Progress = progress;

                    info.DownloadStatus = info.GetStatus();

                    downloadList.Add(info);
                }

            }
            return downloadList;
        }
    }
}
