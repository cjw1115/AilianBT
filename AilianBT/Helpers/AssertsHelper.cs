using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AilianBT.Helpers
{
    public class AssertsHelper
    {
        public static async Task<ImageSource> GetRandomBackgroundImage()
        {
            var imageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets//images//background/{DateTime.Now.Millisecond % 3 + 1}.png"));
            if (imageFile != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (var ras = await imageFile.OpenReadAsync())
                {
                    bitmap.SetSource(ras);
                }
                return bitmap;
            }
            return null;
        }
    }
}
