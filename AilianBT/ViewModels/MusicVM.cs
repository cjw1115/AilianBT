﻿using AilianBT.Services;
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
    public class MusicVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<MusicModel> MusicList { get; set; } = new ObservableCollection<MusicModel>();
        MusicService musicService = new MusicService();
        
        CoreDispatcher _coreDispatcher;
        private MusicManager musicManager;
        public MusicVM(CoreDispatcher coreDispatcher) : this()
        {
            _coreDispatcher = coreDispatcher;
        }
        System.Threading.SynchronizationContext SynchronizationContext = System.Threading.SynchronizationContext.Current;
        public MusicVM()
        {
            musicManager = new MusicManager();
            musicManager.MediaFailed += MusicManager_MediaFailed;
            musicManager.MediaLoaded += MusicManager_MediaLoaded;
            musicManager.MediaChanged += MusicManager_MediaChanged;
        }

        private void MusicManager_MediaChanged(MusicModel newModel, MusicModel oldModel)
        {
            SynchronizationContext.Post((o) =>
            {
                if (newModel != null)
                {
                    if (IsLoading == true)
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

        private void MusicManager_MediaLoaded(MusicModel obj)

        {
            
        }

        private void MusicManager_MediaFailed(MusicModel obj)
        {
            
        }

        public async void Load()
        {
            await Init();
        }
        public void CacheMusic(int? index=null)
        {
            var value = index == null ? CurrentIndex : index.Value;
            if (value < 0 || value >= MusicList.Count)
                return;

            if(value == 0)
            {
                CanPreviou = false;
            }
            else if(value == MusicList.Count-1)
            {
                CanNext = false;
            }
            musicManager.Clear();

            if (value - 1 >= 0)
            {
                CanPreviou = true;
                musicManager.Add(MusicList[value-1]);
            }

            musicManager.Add(MusicList[value]);

            if(MusicList.Count> value + 1)
            {
                CanNext = true;
                musicManager.Add(MusicList[value+1]);
            }
            
            
        }
        public void CacheNextMusic()
        {
            if(CurrentIndex!=0)
            {
                musicManager.Clear(0);
            }
            
            if(CurrentIndex+1<MusicList.Count-1)
            {
                musicManager.Add(MusicList[CurrentIndex + 2]);
            }
            else
            {
                CanPreviou = true;
                CanNext = false;
            }
        }
        public void CachePreviousMusic()
        {
            if (CurrentIndex != MusicList.Count-1)
            {
                musicManager.Clear(2);
            }
            
            if (CurrentIndex - 1 >0)
            {
                musicManager.Add(MusicList[CurrentIndex -2],0);
            }
            else
            {
                CanPreviou = false;
                CanNext = true;
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
            set { _currentIndex = value; OnPropertyChanged(); }
        }
        private bool _canPreviou;
        public bool CanPreviou
        {
            get { return _canPreviou; }
            set { _canPreviou = value; OnPropertyChanged(); }
        }
        private bool _canNext;
        public bool CanNext
        {
            get { return _canNext; }
            set { _canNext = value; OnPropertyChanged(); }
        }
        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if(_isPlaying!=value)
                {
                    _isPlaying = value; OnPropertyChanged();
                }
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }
        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }


        public void previou_Click()
        {
            if (CurrentIndex <=0)
            {
                return;
            }
            if (CurrentIndex - 1 >=0)
            {
                CachePreviousMusic();
                musicManager.Previous();
                CurrentIndex--;
            }
        }

        public void play_Click()
        {
            if(isPaused==true)
            {
                isPaused = false;
                IsPlaying = true;
                musicManager.Play();
            }
            else
            {
                IsLoading = true;
                CacheMusic();
                musicManager.Play();
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

        public void next_Click()
        {
            if(CurrentIndex>=MusicList.Count-1)
            {
                return;
            }
            if(CurrentIndex+1<=MusicList.Count-1)
            {
                CacheNextMusic();
                musicManager.Next();
                CurrentIndex++;
            }
            
        }
        public void ItemClick(MusicModel model)
        {
            IsLoading = true;
            CacheMusic();
            musicManager.Play();
        }
    }
}
