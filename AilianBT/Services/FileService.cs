using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.Services
{
    public class FileService
    {
        //private StorageFolder LocalFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public static async Task<byte[]> GetThumbBytes(IStorageFile file)
        {
            StorageItemThumbnail thumb = await ((StorageFile)file).GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 44);

            byte[] buffer = new byte[thumb.Size];
            var ibuffer = buffer.AsBuffer();
            await thumb.ReadAsync(ibuffer, (uint)thumb.Size, Windows.Storage.Streams.InputStreamOptions.None);
            buffer = ibuffer.ToArray();
            return buffer;
        }
        public static async Task<StorageFile> CreaterFile(StorageFolder folder,string filename)
        {
            //检测文件名出现的特殊字符 /\:*"<>|
            char[] specilChar = new char[] { '/', '\\', '*', '"', '<', '>', '|' };
            var query = filename.Select(m =>
              {
                  if (specilChar.Contains(m))
                      return '-';
                  else
                      return m;
              });
            filename = String.Join(string.Empty, query);
            var file=await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            return file;
        }
        public static async Task<StorageFile> GetFile(string filepath)
        {
            return  await StorageFile.GetFileFromPathAsync(filepath);
        }
        public static async Task DeleteFile(StorageFile file)
        {
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
        public static StorageFolder DefaultDownloadFolder
        {
            get { return Windows.Storage.ApplicationData.Current.LocalFolder; }
        }
        public static async Task<StorageFolder> GetDownloadFolder()
        {
            try
            {
                var path = GetLocalSetting<string>("downloadfolder");
                var re = await StorageFolder.GetFolderFromPathAsync(path);
                return re ?? DefaultDownloadFolder;
            }
            catch
            {
                return DefaultDownloadFolder;
            }
        }
            
           
        public static  void SetDownloadFolder(StorageFolder folder)
        {
            SetLocalSetting<string>("downloadfolder",folder.Path);
            StorageApplicationPermissions.FutureAccessList.Add(folder);
        }
        public static T GetLocalSetting<T>(string key) where T : class
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object value;
            settings.Values.TryGetValue(key, out value);
            return value as T;
        }
        public static void SetLocalSetting<T>(string key, T value) where T : class
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[key] = value;
        }
    }
}
