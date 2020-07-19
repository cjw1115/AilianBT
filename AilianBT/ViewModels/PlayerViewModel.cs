using GalaSoft.MvvmLight;
using AilianBT.Models;
using AilianBT.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

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

            _musicManager.MediaEnd += _musicManagerMediaEnd;
        }

        private void _musicManagerMediaEnd()
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

        public void SetMusicList(Collection<MusicModel> musicList)
        {
            _musicList = musicList;
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
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

        private bool _seeking = false;
        public bool Seeking
        {
            get { return _seeking; }
            set { Set(ref _seeking, value); }
        }

        public async void PlayClicked()
        {
            if (Status == PlayerStatus.Paused)
            {
                Status = PlayerStatus.Playing;
                await _musicManager.Play(_musicList[CurrentIndex]);
            }
            else
            {
                IsLoading = true;
                await _musicManager.Play(_musicList[CurrentIndex]);
                CurrentMusic = _musicList[CurrentIndex];
                IsLoading = false;
                Status = PlayerStatus.Playing;
            }
        }

        public void PauseClicked()
        {
            Status = PlayerStatus.Paused;
            _musicManager.Pause();
        }

        public async void PreviousClicked()
        {
            if (CurrentIndex <= 0)
            {
                return;
            }
            if (CurrentIndex - 1 >= 0)
            {
                CurrentIndex--;
                Status = PlayerStatus.Stopped;
                IsLoading = true;
                await _musicManager.Next(_musicList[CurrentIndex]);
                CurrentMusic = _musicList[CurrentIndex];
                IsLoading = false;
                Status = PlayerStatus.Playing;
            }
        }

        public async void NextClicked()
        {
            if (CurrentIndex >= _musicList.Count - 1)
            {
                return;
            }
            if (CurrentIndex + 1 <= _musicList.Count - 1)
            {
                CurrentIndex++;
                Status = PlayerStatus.Stopped;
                IsLoading = true;
                await _musicManager.Next(_musicList[CurrentIndex]);
                CurrentMusic = _musicList[CurrentIndex];
                IsLoading = false;
                Status = PlayerStatus.Playing;
            }
        }

        public async void Seek(double targetProgress)
        {
            var targetPosition = targetProgress * CurrentMusic.Length;
            await _musicManager.Seek(targetPosition);
        }
        #region Manager events
        private void MusicManager_MediaEnd()
        {
            SynchronizationContext.Post((o) =>
            {
                NextClicked();
            }, null);
        }

        private void MusicManager_MediaCached(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                var matchedModel = _musicList.Where(m => m.Equals(model)).FirstOrDefault();
                matchedModel.HasCached = true;
                //if (model.Equals(_musicList[CurrentIndex]))
                //{
                //    //IsLoading = false;
                //}
            }, null);
        }

        private void MusicManager_MediaLoading(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                if (model.Equals(_musicList[CurrentIndex]))
                {
                    IsLoading = true;
                }
            }, null);

        }

        private void MusicManager_MediaChanged(MusicModel newModel, MusicModel oldModel)
        {
            SynchronizationContext.Post((o) =>
            {

                if (newModel != null)
                {
                    if (newModel.Equals(_musicList[CurrentIndex]))
                    {
                        IsLoading = false;
                    }
                }
                else
                {
                }
            }, null);
        }

        private void MusicManager_MediaLoaded(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                var matchedModel = _musicList.Where(m => m.Equals(model)).FirstOrDefault();
                matchedModel.HasCached = true;
                //if (model.Equals(_musicList[CurrentIndex]))
                //{
                //    //IsLoading = false;
                //}
            }, null);
        }

        private void MusicManager_MediaFailed(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                IsLoading = false;
            }, null);
        }
        #endregion
    }
}

