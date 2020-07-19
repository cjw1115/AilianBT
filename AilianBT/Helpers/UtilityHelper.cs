using AilianBT.Constant;
using AilianBT.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace AilianBT.Helpers
{
    public class UtilityHelper
    {
        private MD5 _md5 = MD5.Create();
        private StringBuilder _md5StrBudilder = new StringBuilder(32);

        public string CreateMd5HashString(string content)
        {
            _md5StrBudilder.Clear();
            byte[] stringbuffer = Encoding.UTF8.GetBytes(content);
            byte[] hashBuffer = _md5.ComputeHash(stringbuffer);

            for (int i = 0; i < hashBuffer.Length; i++)
            {
                _md5StrBudilder.Append(hashBuffer[i].ToString("x2"));
            }
            return _md5StrBudilder.ToString();
        }

        public string ReplaceInvalidCharactorsInFileName(string fileName, char preferredChar = '_')
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                string invalid = new string(Path.GetInvalidFileNameChars());
                foreach (char c in invalid)
                {
                    fileName = fileName.Replace(c, preferredChar);
                }
            }
            return fileName;
        }

        public WindowMode GetWindowMode()
        {
            var currentWidth = Window.Current.Bounds.Width;
            if (currentWidth > Definition.WINDOW_MODE_WIDE_WIDTH)
            {
                return WindowMode.Wide;
            }

            if (ViewModels.ViewModelLocator.Instance.NavigationVM.DetailFrame.BackStackDepth >= 1)
            {
                return WindowMode.Detail;
            }

            return WindowMode.Master;
        }

        public void RunAtUIThread(DispatchedHandler action)
        {
            Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, action);
        }
    }
}
