using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace AilianBT.Converters
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
