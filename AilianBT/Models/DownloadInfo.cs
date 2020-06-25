using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

namespace AilianBT.Models
{
    public class DownloadInfo :ViewModelBase
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private DownloadOperation _downloadOperation;
        public DownloadOperation DownloadOperation
        {
            get { return _downloadOperation; }
            set { Set(ref _downloadOperation, value); }
        }

        private DownloadStatus _downloadStatus;
        public DownloadStatus DownloadStatus
        {
            get { return _downloadStatus; }
            set { Set(ref _downloadStatus , value); }
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
        public DownloadStatus GetStatus()
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
            set { Set(ref _receivedBytes , value); }
        }

        private ulong _totalToReceive;
        public ulong TotalToReceive
        {
            get { return _totalToReceive; }
            set { Set(ref _totalToReceive, value); }
        }

        private double _finishedPercent;
        public double FinishedPercent
        {
            get { return _finishedPercent; }
            set { Set(ref _finishedPercent, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { Set(ref _fileName, value); }
        }

        public byte[] _thumb;
        public byte[] Thumb
        {
            get { return _thumb; }
            set { Set(ref _thumb, value); }
        }

        private CancellationTokenSource _cts;
        public CancellationTokenSource Cts
        {
            get { return _cts; }
            set { Set(ref _cts, value); }
        }

        public IProgress<DownloadOperation> Progress { get; set; }

        private bool? _isSelected = false;
        public bool? IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }
    }
}
