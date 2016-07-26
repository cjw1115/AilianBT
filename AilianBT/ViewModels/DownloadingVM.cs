using BtDownload.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BtDownload.VIewModels
{
    public class DownloadingVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DownloadingVM()
        {
            DownloadOperations = new ObservableCollection<DownloadInfo>();
        }

        private bool _isEnableStart = false;
        public bool IsEnableStart
        {
            get { return _isEnableStart; }
            set { _isEnableStart = value; OnPropertyChanged(); }
        }

        private bool _isEnablePause = false;
        public bool IsEnablePause
        {
            get { return _isEnablePause; }
            set { _isEnablePause = value; OnPropertyChanged(); }
        }

        private bool _isEnableRemove = false;
        public bool IsEnableRemove
        {
            get { return _isEnableRemove; }
            set { _isEnableRemove = value; OnPropertyChanged(); }
        }

        private bool _isEnableSelect = false;
        public bool IsEnableSelect
        {
            get { return _isEnableSelect; }
            set { _isEnableSelect = value; OnPropertyChanged(); }
        }

        private bool _isSelecting = false;
        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { _isSelecting = value;OnPropertyChanged(); }
        }

        public ObservableCollection<DownloadInfo> DownloadOperations { get; set; }
    }
}
