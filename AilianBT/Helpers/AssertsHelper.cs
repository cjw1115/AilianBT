using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AilianBT.Helpers
{
    public class AssertsHelper
    {
        public static async Task<ImageSource> GetRandomBackgroundImage()
        {
            try
            {
                var firstFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets//images//background/1.png"));
                var backgroundFolder = System.IO.Path.GetDirectoryName(firstFile.Path);
                var backgrounds = System.IO.Directory.GetFiles(backgroundFolder);
               
                var selectedBackground = backgrounds[DateTime.Now.Millisecond % backgrounds.Length];
                if (selectedBackground != null)
                {
                    var backgroundFile = await StorageFile.GetFileFromPathAsync(selectedBackground);
                    BitmapImage bitmap = new BitmapImage();
                    using (var ras = await backgroundFile.OpenReadAsync())
                    {
                        bitmap.SetSource(ras);
                    }
                    return bitmap;
                }
            }
            catch 
            {
                // TODO: Log error message
            }
            return null;
        }
    }
}
