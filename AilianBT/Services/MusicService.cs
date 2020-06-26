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
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class MusicService
    {
        private const string KISSSUB_PLAYLIST = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private const string KISSSUB_HOST = "http://www.kisssub.org/";

        private HttpService _httpService = new HttpService(true);
        private UtilityHelper _utilityHelper;

        public MusicService(UtilityHelper utilityHelper)
        {
            _utilityHelper = utilityHelper;
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
            catch(Exception)
            {
                // TODO: log error message
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

        public async Task<bool> CahcheMusicAsync(IRandomAccessStream stream, MusicModel model)
        {
            var buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);
            stream.Seek(0);
            await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);
            StorageFolder folder = null;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (System.IO.FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
            }

            if (folder == null)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CachedMusic");
            }

            StorageFile cacheFile = null;
            try
            {
                cacheFile = await folder.CreateFileAsync(_utilityHelper.CreateMd5HashString(model.Title), CreationCollisionOption.FailIfExists);
            }
            catch (ArgumentNullException)
            {
                //
            }
            catch (System.Exception)
            {
                return true;
            }
            IRandomAccessStream cacheFileStream = null;
            try
            {
                cacheFileStream = await cacheFile.OpenAsync(FileAccessMode.ReadWrite);
                await cacheFileStream.WriteAsync(buffer);
                await cacheFileStream.FlushAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                cacheFileStream?.Dispose();
            }
        }

        public async Task CahcheMusic(Stream sourceStream, MusicModel model)
        {
            var cacheFile = await _createMusicFile(model);
            if (cacheFile == null)
            {
                // TODO: Log error message
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
                        while (cachedSize <= totalSize)
                        {
                            var readSize = sourceStream.Read(tempBuffer, 0, tempBuffer.Length);
                            fileStream.Write(tempBuffer, 0, readSize);
                            fileStream.Flush();
                            cachedSize += readSize;
                        }
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch
            {
                // TODO: Log error message
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
            catch (System.IO.FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
                return null;
            }

            StorageFile cachedFile = null;
            try
            {
                cachedFile = await folder.GetFileAsync(_utilityHelper.CreateMd5HashString(model.Title));
                var ras = await cachedFile.OpenAsync(FileAccessMode.Read);
                return ras.AsStreamForRead();
            }
            catch (System.Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{e.Message}");
#endif
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
            catch (FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
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
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Exception:{e.Message}");
#endif
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
            catch (System.IO.FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
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
            catch (System.Exception)
            {
                // TODO: Log error message
            }
            return cacheFile;
        }

        
    }
}
