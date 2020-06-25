using GalaSoft.MvvmLight;
using System;

namespace AilianBT.Models
{
    public class DownloadedInfo :ViewModelBase
    {
        public  int ID { get; set; }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { Set(ref _fileName, value); }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { Set(ref _filePath, value);}
        }

        private ulong _size;
        public ulong Size
        {
            get { return _size; }
            set { Set(ref _size, value); }
        }

        private DateTime _downloadedTime;
        public DateTime DownloadedTime
        {
            get { return _downloadedTime; }
            set { Set(ref _downloadedTime, value); }
        }

        private byte[] _thumb;
        public byte[] Thumb
        {
            get { return _thumb; }
            set { Set(ref _thumb, value);}
        }

    }
}
