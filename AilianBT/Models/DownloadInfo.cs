using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.Models
{
    public class DownloadInfo : INotifyPropertyChanged
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private DownloadOperation _downloadOperation;

        public DownloadOperation DownloadOperation
        {
            get { return _downloadOperation; }
            set
            {
                _downloadOperation = value;
                OnPropertyChanged();
            }
        }

        private DownloadStatus _downloadStatus;
        public DownloadStatus DownloadStatus
        {
            get { return _downloadStatus; }
            set
            {
                _downloadStatus = value;
                //if (value == DownloadSatus.Pause)
                //{
                //    DownloadOperation.Pause();
                //}
                //else if(value== DownloadSatus.Run)
                //{
                //    DownloadOperation.Resume();
                //}
                OnPropertyChanged();
            }
        }

        public Task AttachAsync(CancellationTokenSource cts, IProgress<DownloadOperation> progress)
        {
            Cts = cts;
            Progress = progress;
            return AttachAsync();
        }
        public Task AttachAsync()
        {
            DownloadStatus = GetStatus();
            return DownloadOperation.AttachAsync().AsTask(Cts.Token, Progress);
        }
        public Task Start(CancellationTokenSource cts,IProgress<DownloadOperation> progress)
        {
            Cts = cts;
            Progress = progress;
            return Start();
        }
        public Task Start()
        {
            DownloadStatus = DownloadStatus.Run;
            return DownloadOperation.StartAsync().AsTask(Cts.Token, Progress);
        }
        public void Resume()
        {
            DownloadOperation.Resume();
            DownloadStatus = DownloadStatus.Run;
        }
        public void Pause()
        {
            DownloadOperation.Pause();
            DownloadStatus = DownloadStatus.Pause;
        }
        public  DownloadStatus GetStatus()
        {
            switch (DownloadOperation.Progress.Status)
            {

                case BackgroundTransferStatus.Running:
                    return DownloadStatus.Run;
                case BackgroundTransferStatus.PausedSystemPolicy:
                case BackgroundTransferStatus.PausedByApplication:
                case BackgroundTransferStatus.PausedCostedNetwork:
                case BackgroundTransferStatus.PausedNoNetwork:
                    return DownloadStatus.Pause;
                case BackgroundTransferStatus.Completed:
                    return DownloadStatus.Complate;
                case BackgroundTransferStatus.Canceled:
                case BackgroundTransferStatus.Error:
                default:
                    return DownloadStatus.Other ;
            }
        }
        private bool IsPasused(BackgroundTransferStatus status)
        {
            switch (status)
            {
                case BackgroundTransferStatus.PausedSystemPolicy:
                case BackgroundTransferStatus.PausedByApplication:
                case BackgroundTransferStatus.PausedCostedNetwork:
                case BackgroundTransferStatus.PausedNoNetwork:
                    return true;
                case BackgroundTransferStatus.Running:
                case BackgroundTransferStatus.Completed:
                case BackgroundTransferStatus.Canceled:
                case BackgroundTransferStatus.Error:
                default:
                    return false;
            }
        }
        private bool IsRuning(BackgroundTransferStatus status)
        {
            switch (status)
            {

                case BackgroundTransferStatus.Running:
                    return true;
                case BackgroundTransferStatus.PausedSystemPolicy:
                case BackgroundTransferStatus.PausedByApplication:
                case BackgroundTransferStatus.PausedCostedNetwork:
                case BackgroundTransferStatus.PausedNoNetwork:
                case BackgroundTransferStatus.Completed:
                case BackgroundTransferStatus.Canceled:
                case BackgroundTransferStatus.Error:
                default:
                    return false;
            }
        }


        private ulong _receivedBytes;

        public ulong ReceivedBytes
        {
            get { return _receivedBytes; }
            set { _receivedBytes = value; OnPropertyChanged(); }
        }
        private ulong _totalToReceive;

        public ulong TotalToReceive
        {
            get { return _totalToReceive; }
            set { _totalToReceive = value; OnPropertyChanged(); }
        }

        private double _finishedPercent;

        public double FinishedPercent
        {
            get { return _finishedPercent; }
            set { _finishedPercent = value; OnPropertyChanged(); }
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }

        public byte[] _thumb;
        public byte[] Thumb
        {
            get { return _thumb; }
            set { _thumb = value;OnPropertyChanged(); }
        }
        private CancellationTokenSource _cts;

        public CancellationTokenSource Cts
        {
            get { return _cts; }
            set { _cts = value; }
        }

        public IProgress<DownloadOperation> Progress { get; set; }
        private bool? _isSelected = false;

        public bool? IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value;OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public enum DownloadStatus
    {
        NoStart,
        Pause,
        Run,
        Complate,
        Other
        
    }
}
