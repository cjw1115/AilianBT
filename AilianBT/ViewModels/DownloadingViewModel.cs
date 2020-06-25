using AilianBT.Models;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace AilianBT.ViewModels
{
    public class DownloadingViewModel: ViewModelBase
    {
        private bool _isEnableStart = false;
        public bool IsEnableStart
        {
            get { return _isEnableStart; }
            set { Set(ref _isEnableStart, value);}
        }

        private bool _isEnablePause = false;
        public bool IsEnablePause
        {
            get { return _isEnablePause; }
            set { Set(ref _isEnablePause, value);}
        }

        private bool _isEnableRemove = false;
        public bool IsEnableRemove
        {
            get { return _isEnableRemove; }
            set { Set(ref _isEnableRemove, value); }
        }

        private bool _isEnableSelect = false;
        public bool IsEnableSelect
        {
            get { return _isEnableSelect; }
            set { Set(ref _isEnableSelect, value);}
        }

        private bool _isSelecting = false;
        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { Set(ref _isSelecting, value); }
        }

        public ObservableCollection<DownloadInfo> _downloadOperations = new ObservableCollection<DownloadInfo>();
        public ObservableCollection<DownloadInfo> DownloadOperations
        {
            get { return _downloadOperations; }
            set { Set(ref _downloadOperations, value); }
        }
    }
}
