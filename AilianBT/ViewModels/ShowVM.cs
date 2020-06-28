using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace AilianBT.ViewModels
{
    public class ShowVM:ViewModelBase
    {
        private AilianBTService _ailianBTService = SimpleIoc.Default.GetInstance<AilianBTService>();
        private DownloadService _downloadService = SimpleIoc.Default.GetInstance<DownloadService>();
        private LogService _logServie = SimpleIoc.Default.GetInstance<LogService>();
        private SettingService _settingService = SimpleIoc.Default.GetInstance<SettingService>();

        private ShowModel _showModel;
        public ShowModel ShowModel
        {
            get { return _showModel; }
            set { Set(ref _showModel, value); }
        }


        public async void Loaded(object parm)
        {
            ShowModel = null;

            var model = parm as AilianResModel;
            ShowModel show = new ShowModel();
            show.Title = model.Title;
            show.PublishTime = model.PostTime;
            show.Uper = model.Author;
            try
            {
                IsLoadingWebView = true;

                var re = await GetDetailInfo(model.Link);
                if (re != null)
                {
                    show.Summary = re.Summary;
                    show.FileInfo = re.FileInfo;
                    show.BtLink = re.BtLink;
                    show.MagnetLink = re.MagnetLink;
                }
                
            }
            catch(Exceptions.NetworkException networkException)
            {
                _logServie.Error("", networkException);
                App.ShowNotification(networkException.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                _logServie.Error("", resolveException);
                App.ShowNotification(resolveException.Message);
            }

            ShowModel = show;
        }

        public async Task<ShowModel> GetDetailInfo(string showUri)
        {
            return await _ailianBTService.GetDetailHtml(showUri);
        }

        public void DownloadBt()
        {
            if (ShowModel != null)
            {
                Uri uri = new Uri(ShowModel.BtLink);
                _downloadService.CreateBackDownload(ShowModel.Title + ".torrent", uri);

                App.ShowNotification("已加入下载队列");
            }
            else
            {
                _logServie.Warning("BT link is not prepared");
            }
        }

        public void DownloadMagnet()
        {
            if (ShowModel != null)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(ShowModel.MagnetLink);
                Clipboard.SetContent(dataPackage);

                App.ShowNotification("复制成功");

                var status = _settingService.GetMagnetAutoDownloadStatus();
                if (status != null && status.Value)
                {
                    var protocal = _settingService.GetMagnetAutoDownloadProtocal();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Launcher.LaunchUriAsync(new Uri(protocal));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            else
            {
                _logServie.Warning("Magnet link is not prepared");
            }
        }

        private bool isLoadingWebView = true;
        public bool IsLoadingWebView
        {
            get => isLoadingWebView;
            set => Set(ref isLoadingWebView, value);
        }
    }
}
