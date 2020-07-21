using AilianBT.Common.Services;
using AilianBT.Constant;
using AilianBT.Helpers;
using AilianBT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class MusicService
    {
        private const string KISSSUB_PLAYLIST = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private const string KISSSUB_HOST = "http://www.kisssub.org/";
        private const string LOCAL_PLAYLIST_FILENAME = "Playlist.json";

        private HttpService _httpService = new HttpService(true);
        private UtilityHelper _utilityHelper;
        private LogService _logger;
        private StorageService _storageService;

        private BackgroundDownloader _backgroundDownloader;
        private BackgroundTransferGroup _backgroundDownloaderGroup = BackgroundTransferGroup.CreateGroup(Definition.MUSIC_DOWNLOAD_GROUP_NAME);

        public MusicService(UtilityHelper utilityHelper,LogService logger, StorageService storageService)
        {
            _utilityHelper = utilityHelper;
            _logger = logger;
            _storageService = storageService;

            _initDownloader();
        }

        #region Playlist
        public async Task<IList<MusicModel>> GetNetPlayListAsync()
        {
            var musicList = new List<MusicModel>();
            try
            {
                var playListUrl = KISSSUB_PLAYLIST + DateTime.Now.Millisecond.ToString();
                var re = await _httpService.SendRequestForString(playListUrl, HttpMethod.Get, Encoding.UTF8);
                if (re != null)
                {
                    var startStr = "shuffle(";
                    var endStr = "));";
                    var startIndex = re.IndexOf(startStr) + startStr.Length;
                    var endIndex = re.IndexOf(endStr);

                    var jsonString = re.Substring(startIndex, endIndex - startIndex);
                    var playlist = JsonSerializer.Deserialize<IList<PlaylistItemModel>>(jsonString);
                    for (int i = 0; i < playlist.Count; i++)
                    {
                        var model = new MusicModel()
                        {
                            ID = i,
                            Title = playlist[i].Title,
                            Uri = new Uri(playlist[i].Url)
                        };
                        musicList.Add(model);
                    }
                }
            }
            catch(Exception e)
            {
                _logger.Error($"Query music list online", e);
            }
            return musicList;
        }

        public async Task<IList<MusicModel>> GetLocalPlayListAsync()
        {
            try
            {
                var fileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, LOCAL_PLAYLIST_FILENAME);
                var playlistFile = await _storageService.GetFile(fileName);
                using(var stream = await playlistFile.OpenStreamForReadAsync())
                {
                    return await JsonSerializer.DeserializeAsync<IList<MusicModel>>(stream);
                }
            }
            catch(FileNotFoundException)
            {
                _logger.Warning($"Didn't find the local playlist file");
            }
            catch
            {
                _logger.Warning($"Parse local playlist file failed");
            }
            return null;
        }

        public async Task CachePlayListAsync(IList<MusicModel> playlist)
        {
            try
            {
                var fileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, LOCAL_PLAYLIST_FILENAME);
                var playlistFile = await _storageService.GetFile(fileName);
                await _storageService.DeleteFile(playlistFile);   
            }
            catch (FileNotFoundException)
            {
                _logger.Information($"Didn't find the local playlist file");
            }

            try
            {
                var playlistFile = await _storageService.CreaterFile(ApplicationData.Current.LocalFolder, LOCAL_PLAYLIST_FILENAME);
                using (var stream = await playlistFile.OpenStreamForWriteAsync())
                {
                    await JsonSerializer.SerializeAsync(stream, playlist);
                }
            }
            catch(Exception e)
            {
                _logger.Error($"Cache playlist file failed", e);
            }
        }
        #endregion

        private void _initDownloader()
        {
            _backgroundDownloader = new BackgroundDownloader();
            _backgroundDownloader.TransferGroup = _backgroundDownloaderGroup;
            _backgroundDownloader.SetRequestHeader("Referer", KISSSUB_HOST);
        }

        public async Task<MediaSource> RequestMusic(MusicModel model)
        {
            EventHandler<DownloadOperation> downloadProgressHandler = (o, e) =>
            {
                _logger.Debug($"Downloading status of {model.Title}:\n\t{e.Progress.Status}, ({e.Progress.BytesReceived}/{e.Progress.TotalBytesToReceive})");
                switch (e.Progress.Status)
                {
                    case BackgroundTransferStatus.Completed:
                        _utilityHelper.RunAtUIThread(() => model.HasCached = true);
                        break;
                    default:
                        break;
                }
            };

            IRandomAccessStreamReference ras = null;
            DownloadOperation downloadOperation = null;
            var downloadProgress = new Progress<DownloadOperation>();
            downloadProgress.ProgressChanged += downloadProgressHandler;

            var cacheFile = await _createMusicFile(model);
            if (cacheFile == null)
            {
                var operations = await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(_backgroundDownloaderGroup);
                foreach (var item in operations)
                {
                    if (item.ResultFile is StorageFile cachedFile)
                    {
                        if (cachedFile.Name == _utilityHelper.CreateMd5HashString(model.Title))
                        {
                            downloadOperation = item;
                            break;
                        }
                    }
                }
                if (downloadOperation != null)
                {
                    ras = downloadOperation.GetResultRandomAccessStreamReference();
                    downloadOperation.AttachAsync().AsTask(downloadProgress);
                }
                else
                {
                    _logger.Warning($"Don't find the downloading task for {model.Title}");
                    return null;
                }
            }
            else
            {
                downloadOperation = _backgroundDownloader.CreateDownload(model.Uri, cacheFile);
                downloadOperation.IsRandomAccessRequired = true;
                ras = downloadOperation.GetResultRandomAccessStreamReference();
                downloadOperation.StartAsync().AsTask(downloadProgress);
            }

            var source = MediaSource.CreateFromStreamReference(ras, "audio/mpeg");
            return source;
        }

        /// <summary>
        /// Get music stream from the local cache
        /// </summary>
        public async Task<MediaSource> GetCahchedMusicAsync(MusicModel model)
        {
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (FileNotFoundException)
            {
                _logger.Warning($"Get music's cache folder failed");
                return null;
            }

            try
            {
                var hashName = _utilityHelper.CreateMd5HashString(model.Title);
                var cachedFile = await folder.GetFileAsync(hashName);
                var ras = await cachedFile.OpenAsync(FileAccessMode.Read);
                _logger.Debug($"Music {model.Title}({hashName}) has local cache");
                return MediaSource.CreateFromStream(ras, "audio/mpeg");
            }
            catch
            {
                _logger.Warning($"Get cached music file failed, target music info:{Environment.NewLine}" +
                    $"\tTitle: {model.Title}{Environment.NewLine}" +
                    $"\tUrl: {model.Uri}");
                return null;
            }
        }

        public async Task CheckCachedMusicAsync(IEnumerable<MusicModel> models, SynchronizationContext context)
        {
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (FileNotFoundException)
            {
                _logger.Warning($"Get music' cache folder failed");
                return;
            }
            
            foreach (var item in models)
            {
                try
                {
                    var result = await folder.TryGetItemAsync(_utilityHelper.CreateMd5HashString(item.Title));
                    if (result != null)
                    {
                        context.Post((o) =>
                        {
                            item.HasCached = true;
                        }, null);
                    }
                }
                catch
                {
                    _logger.Warning($"Get cached music file failed, target file info:{Environment.NewLine}" +
                       $"\tTitle: {item.Title}{Environment.NewLine}" +
                       $"\tUrl: {item.Uri}"
                       );
                }
            }
        }

        private async Task<IStorageFile> _createMusicFile(MusicModel model)
        {
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(Definition.LOCALSTATE_CACHEDMUSIC);
            }
            catch (FileNotFoundException)
            {
                _logger.Warning($"Get music' cache folder failed");
            }

            if (folder == null)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(Definition.LOCALSTATE_CACHEDMUSIC);
            }

            StorageFile cacheFile = null;
            try
            {
                cacheFile = await folder.CreateFileAsync(_utilityHelper.CreateMd5HashString(model.Title), CreationCollisionOption.FailIfExists);
            }
            catch (Exception e)
            {
                _logger.Error($"Create music' cache file failed", e);
            }
            return cacheFile;
        }
    }
}
