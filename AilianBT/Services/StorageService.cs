using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace AilianBT.Services
{
    public class StorageService
    {
        private ApplicationDataContainer _container;
        private LogService _logger;
        public StorageService(LogService logger)
        {
            _logger = logger;
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
                _logger.Error($"Didn't find {settingName} in local settings");
            }
            catch(Exception e)
            {
                _logger.Error($"Finding {settingName} in local settings", e);
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
            catch(Exception e)
            {
                _logger.Error($"Add {settingName} in local settings",e);
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
