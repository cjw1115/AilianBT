using BtDownload.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AilianBT.Services
{
    public class NotificationService
    {
        private static ToastNotifier _notifier= ToastNotificationManager.CreateToastNotifier();
        public static  void ShowDownloadFinishedToast(string message)
        {
            var ison = FileService.GetLocalSetting<bool>("toastswitch");
            if (ison == false)
                return;
            try
            {
                string basicNotification = "<toast><visual><binding template=\"ToastGeneric\"><text>下载完成</text><text>" + message + "</text></binding></visual></toast>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(basicNotification);
                ToastNotification notification = new ToastNotification(doc);
                _notifier.Show(notification);
            }
            catch
            {

            }
            
        }
    }
}
