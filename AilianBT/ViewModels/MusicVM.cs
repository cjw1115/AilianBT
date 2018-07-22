using AilianBT.Services;
using AilianBT.Helpers;
using AilianBT.Models;
using AilianBT.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AilianBT.ViewModels
{
    public class MusicVM : ViewModelBase
    {
        private ObservableCollection<MusicModel> _musicList;
        public ObservableCollection<MusicModel> MusicList
        {
            get => _musicList;
            set => Set(ref _musicList, value);
        }
        MusicService musicService = new MusicService();
        
        CoreDispatcher _coreDispatcher;
        private MusicManager musicManager;
        public MusicVM(CoreDispatcher coreDispatcher) : this()
        {
            _coreDispatcher = coreDispatcher;
            MusicList = new ObservableCollection<MusicModel>();
        }
        System.Threading.SynchronizationContext SynchronizationContext = System.Threading.SynchronizationContext.Current;
        public MusicVM()
        {
            musicManager = new MusicManager();
            musicManager.MediaFailed += MusicManager_MediaFailed;
            //musicManager.MediaLoaded += MusicManager_MediaLoaded;
            musicManager.MediaChanged += MusicManager_MediaChanged;
            musicManager.MediaLoading += MusicManager_MediaLoading;
            musicManager.MediaCached += MusicManager_MediaCached;
            musicManager.MediaEnd += MusicManager_MediaEnd;
        }

        private void MusicManager_MediaEnd()
        {
            SynchronizationContext.Post((o) => 
            {
                next_Click();
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
            musicService.CheckCachedMusic(MusicList.ToList(),SynchronizationContext);
        }
        public async Task CacheMusic(int? index=null)
        {
            var value = index == null ? CurrentIndex : index.Value;
            if (value < 0 || value >= MusicList.Count)
                return;

            if (value - 1 >= 0)
            {
                musicManager.CachedMusicList[0] = MusicList[value - 1];
                musicManager.Add(MusicList[value-1],0);
                
            }

            musicManager.CachedMusicList[1] = MusicList[value];
            await musicManager.Add(MusicList[value],1);

            if(MusicList.Count> value + 1)
            {
                musicManager.CachedMusicList[2] = MusicList[value + 1];
                musicManager.Add(MusicList[value+1],2);
            }
        }
        public async Task Init()
        {
            IList<MusicModel> list = null;
            try
            {
                list = await musicService.GetNetPlayList();
                foreach (var item in list)
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
            CanPreviou = true;
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

        public async void play_Click()
        {
            if(isPaused==true)
            {
                isPaused = false;
                IsPlaying = true;
                musicManager.Play(MusicList[CurrentIndex]);
            }
            else
            {
                IsLoading = true;
                await CacheMusic();
                musicManager.Play(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }
            
        }
        /// <summary>
        /// Mark status of pause.
        /// </summary>
        private bool isPaused = false;
        public void pause_Click()
        {
            IsPlaying = false;
            isPaused = true;
            musicManager.Pause();
        }
        public async void previou_Click()
        {
            if (CurrentIndex <= 0)
            {
                return;
            }
            if (CurrentIndex - 1 >= 0)
            {
                CurrentIndex--;
                await CacheMusic();
                musicManager.Previous(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }
        }
        public async void next_Click()
        {
            if(CurrentIndex>=MusicList.Count-1)
            {
                return;
            }
            if(CurrentIndex+1<=MusicList.Count-1)
            {
                CurrentIndex++;
                await CacheMusic();
                musicManager.Next(MusicList[CurrentIndex]);
                Title = MusicList[CurrentIndex].Title;
            }
            
        }
        public async void ItemClick(MusicModel model)
        {
            IsLoading = true;

            musicManager.Pause(true);

            await CacheMusic();
            musicManager.Play(model);
            Title = model.Title;
        }
    }
}

