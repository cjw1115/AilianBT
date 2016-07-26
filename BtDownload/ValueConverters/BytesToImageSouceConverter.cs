using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.ValueConverters
{
    public class BytesToImageSouceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[] byteBuffer = (byte[])value;
            BitmapImage bitmap = new BitmapImage();

            using (InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream())
            {
                using (var stream = ras.AsStreamForWrite())
                {
                    stream.Write(byteBuffer, 0, byteBuffer.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    bitmap.SetSource(stream.AsRandomAccessStream());
                }
            }
            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
