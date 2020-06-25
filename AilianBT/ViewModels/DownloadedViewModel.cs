using AilianBT.Models;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace AilianBT.ViewModels
{
    public class DownloadedViewModel : ViewModelBase
    {
        private ObservableCollection<DownloadedInfo> _downloadedInfoList = new ObservableCollection<DownloadedInfo>();
        public ObservableCollection<DownloadedInfo> DownloadedInfoList
        {
            get { return _downloadedInfoList; }
            set { Set(ref _downloadedInfoList, value); }
        }

        private bool _isEnableRemove = false;
        public bool IsEnableRemove
        {
            get { return _isEnableRemove; }
            set { Set(ref _isEnableRemove , value);}
        }

        private bool _isEnableSelect = false;
        public bool IsEnableSelect
        {
            get { return _isEnableSelect; }
            set { Set(ref _isEnableSelect, value); }
        }

        private bool _isSelecting = false;
        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { Set(ref _isSelecting, value); }
        }
    }
}
