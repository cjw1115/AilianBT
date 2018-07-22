using AilianBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class MusicService
    {
        private readonly string listUri = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private readonly string kisssUbUri = "http://www.kisssub.org/";
        private static HttpBaseService _httpService = new HttpBaseService(true);
        public async Task<IList<MusicModel>> GetNetPlayList()
        {
            List<MusicModel> musicList = new List<MusicModel>();
            var re=await _httpService.SendRequst(listUri+DateTime.Now.Millisecond.ToString());
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
            var re = await _httpService.GetUriStream(newUri, referUri: kisssUbUri);
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
                cacheFile = await folder.CreateFileAsync(model.Title, Windows.Storage.CreationCollisionOption.FailIfExists);
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
                cachedFile = await folder.GetFileAsync(model.Title);
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

        private string CreateFileName(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                throw new ArgumentNullException("Filename should't be nullable");

            string strAlgName = HashAlgorithmNames.Sha1;
            
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

            CryptographicHash objHash = objAlgProv.CreateHash();

            
            IBuffer buffMsg1 = CryptographicBuffer.ConvertStringToBinary(arg.Trim(), BinaryStringEncoding.Utf16BE);
            objHash.Append(buffMsg1);
            IBuffer buffHash1 = objHash.GetValueAndReset();
            string strHash1 = CryptographicBuffer.EncodeToBase64String(buffHash1);
            return strHash1;
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
    }
}
