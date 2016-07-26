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
        private static BackgroundDownloader _downloader = new BackgroundDownloader();

        public DownloadService()
        {
            
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
        public static async Task<DownloadedInfo> FinishedDownload(DownloadInfo downloadInfo)
        {
            DownloadedInfo downloadedInfo = new DownloadedInfo();
            downloadedInfo.DownloadedTime = DateTime.Now;
            downloadedInfo.FileName = downloadInfo.FileName;
            downloadedInfo.FilePath = downloadInfo.DownloadOperation.ResultFile.Path;
            downloadedInfo.Size = downloadInfo.TotalToReceive;

            var file = downloadInfo.DownloadOperation.ResultFile;
            downloadedInfo.Thumb = await FileService.GetThumbBytes(file);

            return downloadedInfo;
        }


        public async static void CreateBackDownload(Uri uri)
        {
            var filename = GetDownloadFileName(uri.ToString());
            var folder =await  FileService.GetDownloadFolder();
            var file = await FileService.CreaterFile(folder, filename);
            var operation = _downloader.CreateDownload(uri, file);
            await  operation.StartAsync();
        }
        public async static void CreateBackDownload(string filename,Uri uri)
        {
            var folder = await FileService.GetDownloadFolder();
            var file = await FileService.CreaterFile(folder, filename);
            var operation = _downloader.CreateDownload(uri, file);
            await operation.StartAsync();


            var thumb = await FileService.GetThumbBytes(file);
            DownloadedInfo info = new DownloadedInfo()
            {
                DownloadedTime = DateTime.Now,
                FileName = filename,
                FilePath = file.Path,
                Size = operation.Progress.TotalBytesToReceive,
                Thumb = thumb
            };
            StoreDownloadedInfo(info);
        }

        public static async void StoreDownloadedInfo(DownloadedInfo downloadedInfo)
        {
            var dbcontext = new DownloadDbContext();
            dbcontext.DownloadedInfos.Add(downloadedInfo);
            await dbcontext.SaveChangesAsync();
        }
        public static async void StoreDownloadedInfo(IEnumerable<DownloadedInfo> downloadedInfos)
        {
            var dbcontext = new DownloadDbContext();
            dbcontext.DownloadedInfos.AddRange(downloadedInfos);
            await dbcontext.SaveChangesAsync();
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

        public static Uri GetDownloadUri(string uriString)
        {
            var re = uriString.Trim();
            Uri uri = new Uri(re);
            return uri;
        }

        public static string GetDownloadFileName(string uriString)
        {
            uriString = uriString.Trim();
            var index = uriString.LastIndexOf('/');
            var filename = uriString.Substring(index + 1);
            index = filename.IndexOf('?');
            if (index > 0)
            {
                filename = filename.Substring(0, index);
            }
            return filename;
        }
    }
}
