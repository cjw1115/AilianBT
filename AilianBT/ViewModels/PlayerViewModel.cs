using GalaSoft.MvvmLight;
using AilianBT.Models;
using AilianBT.Services;
using System.Collections.ObjectModel;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace AilianBT.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private SynchronizationContext SynchronizationContext = SynchronizationContext.Current;
        private MusicManager _musicManager = new MusicManager();
        private Collection<MusicModel> _musicList;

        public PlayerViewModel()
        {
            _musicManager.PositionChanged += _playbackPositionChanged;
            _musicManager.MediaLoaded += _musicManagerMediaLoaded;
            _musicManager.MediaEnd += _musicManagerMediaEnd;
            _musicManager.MediaFailed += _musicManagerMediaFailed;
        }

        public void SetMusicList(Collection<MusicModel> musicList)
        {
            _musicList = musicList;

            if (CurrentIndex == -1 && _musicList.Count > 0)
            {
                CurrentIndex = 0;
            }

            if (CurrentIndex != -1)
            {
                CurrentMusic = _musicList[CurrentIndex];
            }
        }

        private bool _seeking = false;
        public bool Seeking
        {
            get { return _seeking; }
            set { Set(ref _seeking, value); }
        }

        private int _currentIndex = -1;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                Set(ref _currentIndex, value);

                if (CurrentIndex <= 0)
                {
                    CanPreviou = false;
                }
                else
                {
                    CanPreviou = true;
                }
                if (CurrentIndex >= _musicList.Count - 1)
                {
                    CanNext = false;
                }
                else
                {
                    CanNext = true;
                }
            }
        }

        #region Player UI related props
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private bool _canPreviou;
        public bool CanPreviou
        {
            get { return _canPreviou; }
            set { Set(ref _canPreviou, value); }
        }

        private PlayerStatus _status = PlayerStatus.Stopped;
        public PlayerStatus Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private bool _canNext;
        public bool CanNext
        {
            get { return _canNext; }
            set { Set(ref _canNext, value); }
        }

        private MusicModel _currentMusic;
        public MusicModel CurrentMusic
        {
            get { return _currentMusic; }
            set { Set(ref _currentMusic, value); }
        }

        private double _playbackProgress = 0;
        public double PlaybackProgress
        {
            get { return _playbackProgress; }
            set { Set(ref _playbackProgress, value); }
        }
        #endregion

        #region Playback control actions
        public async void PlayClicked()
        {
            if (_musicList.Count <= 0 || CurrentIndex == -1)
                return;
            if (Status == PlayerStatus.Paused)
            {
                Status = PlayerStatus.Playing;
                await _musicManager.Play(_musicList[CurrentIndex]);
            }
            else
            {
                await _play();
            }
        }

        public void PauseClicked()
        {
            Status = PlayerStatus.Paused;
            _musicManager.Pause();
        }

        public async void PreviousClicked()
        {
            if (_musicList.Count <= 0)
                return;
            if (CurrentIndex <= 0)
            {
                return;
            }
            if (CurrentIndex - 1 >= 0)
            {
                CurrentIndex--;
                await _play();
            }
        }

        public async void NextClicked()
        {
            if (_musicList.Count <= 0)
                return;
            if (CurrentIndex >= _musicList.Count - 1)
            {
                return;
            }
            if (CurrentIndex + 1 <= _musicList.Count - 1)
            {
                CurrentIndex++;
                await _play();
            }
        }

        private async Task _play()
        {
            PlaybackProgress = 0;
            Status = PlayerStatus.Stopped;
            IsLoading = true;
            await _musicManager.Next(_musicList[CurrentIndex]);
            CurrentMusic = _musicList[CurrentIndex];
            Status = PlayerStatus.Playing;
        }

        public async void Seek(double targetProgress)
        {
            var targetPosition = targetProgress * CurrentMusic.Length;
            await _musicManager.Seek(targetPosition);
        }
        #endregion

        #region Manager events
        private void _musicManagerMediaLoaded(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                IsLoading = false;
            }, null);
        }

        private void _musicManagerMediaFailed(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                IsLoading = false;
                NextClicked();
            }, null);
        }
        
        private void _musicManagerMediaEnd(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                NextClicked();
            }, null);
        }

        private void _playbackPositionChanged(TimeSpan position, TimeSpan length)
        {
            if (!Seeking)
            {
                SynchronizationContext.Post((o) =>
                {
                    CurrentMusic.Position = position;
                    CurrentMusic.Length = length;

                    if (length != TimeSpan.Zero)
                    {
                        PlaybackProgress = (100d) * position / length;
                    }
                }, null);
            }
        }
        #endregion
    }
}