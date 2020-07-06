using GalaSoft.MvvmLight;
using AilianBT.Models;

namespace AilianBT.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        public PlayerViewModel()
        { 
        }

        private int _currentIndex = -1;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                Set(ref _currentIndex, value);
            }
        }

        private bool _canPreviou;
        public bool CanPreviou
        {
            get { return _canPreviou; }
            set { Set(ref _canPreviou, value); }
        }

        private bool _canNext;
        public bool CanNext
        {
            get { return _canNext; }
            set { Set(ref _canNext, value); }
        }

        private PlayerStatus _status =  PlayerStatus.Stopped;
        public PlayerStatus Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        public async void PlayClicked()
        {
            Status = PlayerStatus.Playing;
        }

        public void PauseClicked()
        {
            Status = PlayerStatus.Paused;
        }

        public async void PreviousClicked()
        {
        }

        public async void NextClicked()
        {
        }
    }
}

