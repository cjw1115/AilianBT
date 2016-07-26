using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BtDownload.Models
{
    public class DownloadedInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName=null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        public DownloadedInfo()
        {

        }

        public  int ID { get; set; }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value;OnPropertyChanged(); }
        }

        private ulong _size;

        public ulong Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged(); }
        }

        private DateTime _downloadedTime;

        public DateTime DownloadedTime
        {
            get { return _downloadedTime; }
            set { _downloadedTime = value; OnPropertyChanged(); }
        }

        private byte[] _thumb;
        
        public byte[] Thumb
        {
            get { return _thumb; }
            set { _thumb = value;OnPropertyChanged(); }
        }

    }
}
