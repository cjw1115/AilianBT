using BtDownload.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var re=await GetDetailInfo(model.Link);
            if (re != null)
            {
                show.Summary = re.Summary;
                show.FileInfo = re.FileInfo;
                show.BtLink = re.BtLink;
                show.MagnetLink = re.MagnetLink;
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
            DownloadService.CreateBackDownload(ShowModel.Title+".torrent",uri);
            //var locator = App.Current.Resources["Locator"] as ViewModelLocator;
            //var item=locator.NavigationVM.NavigationList.Where(m => m.PageType == typeof(BtDownload.Views.DownloadMainView)).FirstOrDefault();
            //if(item != null)
            //{
            //    var index=locator.NavigationVM.NavigationList.IndexOf(item);
            //    locator.NavigationVM.SelectedIndex = index;
            //    locator.NavigationVM.SelectionChanged(null, null);
            //}
        }

        public ICommand DownloadMagnetCommand { get; set; }
        public void DownloadMagnet(object param)
        {
            var magnetlink = (string)param;
            DataPackage datapackage = new DataPackage();
            datapackage.SetText(magnetlink);
            Clipboard.SetContent(datapackage);
        }
    }
}
