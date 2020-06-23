﻿using AilianBT.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace AilianBT.ViewModels
{
    public class ShowVM:ViewModelBase
    {
        private BLL.AilianBTBLL bll = new BLL.AilianBTBLL();
        private Models.ShowModel _showModel;
        public Models.ShowModel ShowModel
        {
            get { return _showModel; }
            set { Set(ref _showModel, value); }
        }

        private DownloadService _downloadService = SimpleIoc.Default.GetInstance<DownloadService>();

        public ShowVM()
        {
            DownloadBtCommand = new RelayCommand<object>(DownloadBt);
            DownloadMagnetCommand = new RelayCommand<object>(DownloadMagnet);
        }
        public async void Loaded(object parm)
        {
            var model = parm as Models.AilianResModel;
            Models.ShowModel show = new Models.ShowModel();
            show.Title = model.Title;
            show.PublishTime = model.PostTime;
            show.Uper = model.Author;
            try
            {
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
                App.ShowNotification(networkException.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                App.ShowNotification(resolveException.Message);
            }

            ShowModel = show;
        }
        public async Task<Models.ShowModel> GetDetailInfo(string showUri)
        {
            var re=await bll.GetDetailHtml(showUri);
            return re;
        }

        public ICommand DownloadBtCommand { get; set; }
        public void DownloadBt(object param)
        {
            var link = (string)param;
            Uri uri = new Uri(link);
            _downloadService.CreateBackDownload(ShowModel.Title+".torrent",uri);

            App.ShowNotification("已加入下载队列");
        }

        public ICommand DownloadMagnetCommand { get; set; }
        public void DownloadMagnet(object param)
        {
            var magnetlink = (string)param;
            DataPackage datapackage = new DataPackage();
            datapackage.SetText(magnetlink);
            Clipboard.SetContent(datapackage);

            App.ShowNotification("复制成功");
        }
    }
}
