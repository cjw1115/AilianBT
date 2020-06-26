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
using Windows.Storage;

namespace AilianBT.Services
{
    public class MusicService
    {
        private const string KISSSUB_PLAYLIST = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private const string KISSSUB_HOST = "http://www.kisssub.org/";

        private HttpService _httpService = new HttpService(true);
        private UtilityHelper _utilityHelper;
        private LogService _logger;

        public MusicService(UtilityHelper utilityHelper,LogService logger)
        {
            _utilityHelper = utilityHelper;
            _logger = logger;
        }

        public async Task<IList<MusicModel>> GetNetPlayList()
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

        public async Task<Stream> GetMusicStream(Uri uri)
        {
            var newUri=uri.ToString() + "?date=" + DateTime.Now.Millisecond;
            var headers = new Dictionary<string, string>
            {
                { "Referer", KISSSUB_HOST }
            };

            return await _httpService.SendRequestForStream(newUri, HttpMethod.Get, appendHeaders: headers);
        }

        public async Task CahcheMusic(Stream sourceStream, MusicModel model)
        {
            var cacheFile = await _createMusicFile(model);
            if (cacheFile == null)
            {
                _logger.Error($"Create local file for storing music failed");
                return;
            }

            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var fileStream = await cacheFile.OpenStreamForWriteAsync())
                    {
                        long totalSize = sourceStream.Length;
                        long cachedSize = 0;
                        byte[] tempBuffer = new byte[48000];

                        _logger.Debug($"Start to save audio stream, expected {totalSize} bytes");
                        while (cachedSize <= totalSize)
                        {
                            var readSize = sourceStream.Read(tempBuffer, 0, tempBuffer.Length);
                            fileStream.Write(tempBuffer, 0, readSize);
                            fileStream.Flush();
                            cachedSize += readSize;
                        }
                        _logger.Debug($"Save audio stream successfully, cached {cachedSize} bytes");
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception e)
            {
                _logger.Error($"Caching music:{Environment.NewLine}" +
                    $"\tTitle: {model.Title}{Environment.NewLine}" +
                    $"\tUrl: {model.Uri}"
                    , e);
            }
        }

        /// <summary>
        /// Get music stream from the local cache
        /// </summary>
        public async Task<Stream> GetCahchedMusicAsync(MusicModel model)
        {
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (FileNotFoundException e)
            {
                _logger.Error($"Get music' cache folder failed", e);
                return null;
            }

            try
            {
                var hashName = _utilityHelper.CreateMd5HashString(model.Title);
                var cachedFile = await folder.GetFileAsync(hashName);
                var ras = await cachedFile.OpenAsync(FileAccessMode.Read);
                _logger.Debug($"Music {model.Title}({hashName}) has local cache");
                return ras.AsStreamForRead();
            }
            catch (Exception e)
            {
                _logger.Error($"Get cached music file failed, target music info:{Environment.NewLine}" +
                    $"\tTitle: {model.Title}{Environment.NewLine}" +
                    $"\tUrl: {model.Uri}"
                    , e);
                return null;
            }
        }

        public async Task CheckCachedMusic(IList<MusicModel> models, SynchronizationContext context)
        {
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (FileNotFoundException e)
            {
                _logger.Error($"Get music' cache folder failed", e);
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
                catch (Exception e)
                {
                    _logger.Error($"Get cached music file failed, target file info:{Environment.NewLine}" +
                       $"\tTitle: {item.Title}{Environment.NewLine}" +
                       $"\tUrl: {item.Uri}"
                       , e);
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
            catch (FileNotFoundException e)
            {
                _logger.Error($"Get music' cache folder failed", e);
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
