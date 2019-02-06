﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media;
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
        
        public static async Task<StorageFolder> PickDefaultDownloadFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".ailianbt");
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                FileService.SetDownloadFolder(folder);
                return folder;
            }
            return null;
        }
        public static async Task<StorageFolder> GetDownloadFolder()
        {
            try
            {
                var path = GetLocalSetting<string>("downloadfolder");
                StorageFolder folder = null;
                if (path == null)
                {
                    folder = await PickDefaultDownloadFolder();
                }
                else
                {
                    folder = await StorageFolder.GetFolderFromPathAsync(path);
                }
                return folder;
            }
            catch
            {
                return null;
            }
        }
            
           
        public static  void SetDownloadFolder(StorageFolder folder)
        {
            SetLocalSetting<string>("downloadfolder",folder.Path);
            StorageApplicationPermissions.FutureAccessList.Add(folder);
        }
        public static T GetLocalSetting<T>(string key)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object value;
            settings.Values.TryGetValue(key, out value);
            if (value == null)
                return default(T);
            return (T)value;
        }
        public static void SetLocalSetting<T>(string key, T value)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[key] = value;
        }


        public static async Task<ImageSource> GetBackgroundImage()
        {
            var imageFile=await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///images//background/{DateTime.Now.Millisecond % 3 + 1}.png"));
            if (imageFile != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using(var ras=await imageFile.OpenReadAsync())
                {
                    bitmap.SetSource(ras);
                }
                return bitmap;
            }
            return null;
        }
    }
}
