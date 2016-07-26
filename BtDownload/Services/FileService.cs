using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.Services
{
    public class FileService
    {
        public static async Task<byte[]> GetThumbBytes(IStorageFile file)
        {
            StorageItemThumbnail thumb = await ((StorageFile)file).GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 44);

            byte[] buffer = new byte[thumb.Size];
            var ibuffer = buffer.AsBuffer();
            await thumb.ReadAsync(ibuffer, (uint)thumb.Size, Windows.Storage.Streams.InputStreamOptions.None);
            buffer = ibuffer.ToArray();
            return buffer;
        }
    }
}
