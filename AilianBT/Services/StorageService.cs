using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AilianBT.Services
{
    public class StorageService
    {
        private ApplicationDataContainer _container;

        public StorageService()
        {
            _container = ApplicationData.Current.LocalSettings;
        }

        public T GetLocalSetting<T>(string settingName)
        {
            try
            {
                if (_container.Values.TryGetValue(settingName, out object settingObj))
                {
                    if(settingObj is T setting)
                    {
                        return setting;
                    }
                    return JsonSerializer.Deserialize<T>(settingObj as string);
                }
                // TODO: Log error message
            }
            catch
            {
            }
            return default(T);
        }

        public void SetLocalSetting<T>(string settingName, T setting)
        {
            try
            {
                if(setting is ValueType || setting is string)
                {
                    _container.Values[settingName] = setting;
                }
                else
                {
                    if (setting != null)
                    {
                        _container.Values[settingName] = JsonSerializer.Serialize(setting);
                    }
                }
            }
            catch
            {
            }
        }

        public async Task<byte[]> GetThumbBytes(IStorageFile file)
        {
            using (var thumb = await ((StorageFile)file).GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 44))
            {
                byte[] buffer = new byte[thumb.Size];
                await thumb.ReadAsync(buffer.AsBuffer(), (uint)thumb.Size, Windows.Storage.Streams.InputStreamOptions.None);
                return buffer;
            }
        }

        public async Task<StorageFile> CreaterFile(StorageFolder folder, string filename)
        {
            char[] specilChar = new char[] { '/', '\\', '*', '"', '<', '>', '|' };
            var query = filename.Select(m => specilChar.Contains(m) ? '-' : m);
            filename = string.Join(string.Empty, query);
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            return file;
        }

        public async Task<StorageFile> GetFile(string filepath)
        {
            return await StorageFile.GetFileFromPathAsync(filepath);
        }

        public async Task DeleteFile(StorageFile file)
        {
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
    }
}
