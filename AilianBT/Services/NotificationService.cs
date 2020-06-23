using GalaSoft.MvvmLight.Ioc;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AilianBT.Services
{
    public class NotificationService
    {
        private StorageService _storageService = SimpleIoc.Default.GetInstance<StorageService>();

        private ToastNotifier _notifier= ToastNotificationManager.CreateToastNotifier();

        public void ShowDownloadFinishedToast(string message)
        {
            var ison = _storageService.GetLocalSetting<bool>("toastswitch");
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
