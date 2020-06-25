using AilianBT.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace AilianBT.Common.Services
{
    public class MusicService
    {
        private const string KISSSUB_PLAYLIST = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private const string KISSSUB_HOST = "http://www.kisssub.org/";

        private static HttpService _httpService = new HttpService(true);

        public async Task<IList<MusicModel>> GetNetPlayList()
        {
            List<MusicModel> musicList = new List<MusicModel>();
            var re = await _httpService.SendRequestForString(KISSSUB_PLAYLIST + DateTime.Now.Millisecond.ToString(), HttpMethod.Get, Encoding.UTF8);
            if (re != null)
            {
                var startStr = "shuffle(";
                var endStr = "));";
                var startIndex = re.IndexOf(startStr) + startStr.Length;
                var endIndex = re.IndexOf(endStr);

                var jsonString = re.Substring(startIndex, endIndex - startIndex);

                var arry = JsonArray.Parse(jsonString);
                for (int i=0;i<arry.Count;i++)
                {   
                   MusicModel model = new MusicModel();
                    model.ID = i;
                    model.Title = arry[i].GetObject()["title"].GetString();
                    var uriString = arry[i].GetObject()["url"].GetString();
                    model.Uri = new Uri(uriString);
                    musicList.Add(model);
                }
            }
            return musicList;
        }

        public async Task<IRandomAccessStream> GetMusicStream(Uri uri)
        {
            var newUri=uri.ToString() + "?date=" + DateTime.Now.Millisecond;
            var headers = new Dictionary<string, string>
            {
                { "Referer", KISSSUB_HOST }
            };
            var re = await _httpService.SendRequestForRAS(newUri, HttpMethod.Get, appendHeaders: headers);
            return re;
        }

        public async Task<bool> CahcheMusicAsync(IRandomAccessStream stream, MusicModel model)
        {
            Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);
            stream.Seek(0);
            await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);
            Windows.Storage.StorageFolder folder = null;
            try
            {
                folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (System.IO.FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
            }

            if (folder == null)
            {
                folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("CachedMusic");
            }

            Windows.Storage.StorageFile cacheFile = null;
            try
            {
                cacheFile = await folder.CreateFileAsync(_createFileName(model), Windows.Storage.CreationCollisionOption.FailIfExists);
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
                cacheFileStream = await cacheFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
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

        /// <summary>
        /// Get music stream from the local cache
        /// </summary>
        public async Task<IRandomAccessStream> GetCahchedMusicAsync(MusicModel model)
        {
            Windows.Storage.StorageFolder folder = null;
            try
            {
                folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (System.IO.FileNotFoundException IOException)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{IOException.Message}");
#endif
                return null;
            }

            Windows.Storage.StorageFile cachedFile = null;
            try
            {
                cachedFile = await folder.GetFileAsync(_createFileName(model));
                return await cachedFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            }
            catch (System.Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Exception:{e.Message}");
#endif
                return null;
            }
        }

        public async Task CheckCachedMusic(IList<MusicModel> models,System.Threading.SynchronizationContext context)
        {
            Windows.Storage.StorageFolder folder = null;
            try
            {
                folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync("CachedMusic");
            }
            catch (System.IO.FileNotFoundException IOException)
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
                    var re = await folder.TryGetItemAsync(item.Title);
                    if(re!=null)
                    {
                        context.Post((o) =>
                        {
                            item.HasCached = true;
                        }, null);
                    }
                }
                catch (System.Exception e)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Exception:{e.Message}");
#endif
                }
            }
        }

        private string _createFileName(MusicModel model)
        {
            byte[] sor = Encoding.UTF8.GetBytes(model.Title);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));
            }
            return strbul.ToString();
        }
    }
}
