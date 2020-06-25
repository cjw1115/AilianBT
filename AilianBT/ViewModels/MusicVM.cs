using AilianBT.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using AilianBT.Common.Models;
using AilianBT.Common.Services;
using GalaSoft.MvvmLight;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using GalaSoft.MvvmLight.Ioc;
using System.Threading;
using Windows.Services.Maps;

namespace AilianBT.ViewModels
{
    public class MusicVM : ViewModelBase
    {
        private ObservableCollection<MusicModel> _musicList = new ObservableCollection<MusicModel>();
        public ObservableCollection<MusicModel> MusicList
        {
            get => _musicList;
            set => Set(ref _musicList, value);
        }

        private MusicService _musicService = SimpleIoc.Default.GetInstance<MusicService>();
        private MusicManager _musicManager = null;

        private SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

        public MusicVM()
        { 
            _musicManager = new MusicManager();
            _musicManager.MediaFailed += MusicManager_MediaFailed;
            _musicManager.MediaChanged += MusicManager_MediaChanged;
            _musicManager.MediaLoading += MusicManager_MediaLoading;
            _musicManager.MediaCached += MusicManager_MediaCached;
            _musicManager.MediaEnd += MusicManager_MediaEnd;
        }

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
                var matchedModel = MusicList.Where(m => m.Equals(model)).FirstOrDefault();
                matchedModel.HasCached = true;
                //if (model.Equals(MusicList[CurrentIndex]))
                //{
                //    //IsLoading = false;
                //}
            }, null);
        }

        private void MusicManager_MediaLoading(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                if(model.Equals(MusicList[CurrentIndex]))
                {
                    IsLoading = true;
                    IsPlaying = false;
                }
            }, null);
           
        }

        private void MusicManager_MediaChanged(MusicModel newModel, MusicModel oldModel)
        {
            SynchronizationContext.Post((o) =>
            {
                
                if (newModel != null)
                {
                    if (newModel.Equals(MusicList[CurrentIndex]))
                    {
                        IsLoading = false;
                        IsPlaying = true;
                    }
                }
                else
                {
                    IsPlaying = false;
                    //stop play
                }
            }, null);
        }

        private void MusicManager_MediaLoaded(MusicModel model)
        {
            SynchronizationContext.Post((o) =>
            {
                var matchedModel=MusicList.Where(m => m.Equals(model)).FirstOrDefault();
                matchedModel.HasCached = true;
                //if (model.Equals(MusicList[CurrentIndex]))
                //{
                //    //IsLoading = false;
                //}
            },null);
        }

        private void MusicManager_MediaFailed(MusicModel model)
        {  
        }

        public async void Load()
        {
            await Init();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _musicService.CheckCachedMusic(MusicList.ToList(), SynchronizationContext);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task CacheMusic(int? index = null)
        {
            var value = index == null ? CurrentIndex : index.Value;
            if (value < 0 || value >= MusicList.Count)
                return;

            // Need to cache 3 audio file at one time.
            if (value - 1 >= 0)
            {
                _musicManager.CachedMusicList[0] = MusicList[value - 1];
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _musicManager.Add(MusicList[value-1],0);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            _musicManager.CachedMusicList[1] = MusicList[value];
            await _musicManager.Add(MusicList[value], 1);

            if(MusicList.Count> value + 1)
            {
                _musicManager.CachedMusicList[2] = MusicList[value + 1];
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _musicManager.Add(MusicList[value+1],2);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public async Task Init()
        {
            IList<MusicModel> musicList = null;
            try
            {
                musicList = await _musicService.GetNetPlayList();
                foreach (var item in musicList)
                {
                    MusicList.Add(item);
                }
                CurrentIndex = 0;
            }
            catch(Exception e)
            {
                App.ShowNotification(e.Message);
                return;
            }
            CanPreviou = false;
            CanNext = true;
        }

        private int _currentIndex = -1;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                Set(ref _currentIndex, value);

                if(CurrentIndex<=0)
                {
                    CanPreviou = false;
                }
                else
                {
                    CanPreviou = true;
                }
                if (CurrentIndex >= MusicList.Count - 1)
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

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { Set(ref _isPlaying, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        public async void PlayClicked()
        {
            if (isPaused)
            {
                isPaused = !isPaused;
                IsPlaying = true;
                _musicManager.Play(MusicList[CurrentIndex]);
            }
            else
            {
                IsLoading = true;
                await CacheMusic();
                _musicManager.Play(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }   
        }

        private bool isPaused = false;
        public void PauseClicked()
        {
            IsPlaying = false;
            isPaused = true;
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
                await CacheMusic();
                _musicManager.Previous(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }
        }

        public async void NextClicked()
        {
            if(CurrentIndex>=MusicList.Count-1)
            {
                return;
            }
            if(CurrentIndex+1<=MusicList.Count-1)
            {
                CurrentIndex++;
                await CacheMusic();
                _musicManager.Next(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }
        }

        public async void ItemClicked(MusicModel model)
        {
            IsLoading = true;

            _musicManager.Pause(true);

            await CacheMusic();
            _musicManager.Play(model);
            Title = model.Title;
        }
    }
}

