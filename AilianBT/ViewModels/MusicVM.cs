using AilianBT.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Threading;
using AilianBT.Models;

namespace AilianBT.ViewModels
{
    public class MusicVM : ViewModelBase
    {
        private LogService _logger = SimpleIoc.Default.GetInstance<LogService>();
        private MusicService _musicService = SimpleIoc.Default.GetInstance<MusicService>();
        private SynchronizationContext SynchronizationContext = SynchronizationContext.Current;
        public PlayerViewModel PlayerVM { get; } = ViewModelLocator.Instance.PlayerVM;

        public MusicVM()
        {
        }

        private ObservableCollection<MusicModel> _musicList = new ObservableCollection<MusicModel>();
        public ObservableCollection<MusicModel> MusicList
        {
            get => _musicList;
            set => Set(ref _musicList, value);
        }

        public async void Load()
        {
            await _preparePlaylist();
            PlayerVM.SetMusicList(MusicList);
            await _musicService.CheckCachedMusicAsync(MusicList, SynchronizationContext);
        }

        private async Task _preparePlaylist()
        {
            IList<MusicModel> musicList = null;
            try
            {
                musicList = await _musicService.GetLocalPlayListAsync();
                if (musicList == null)
                {
                    musicList = await _musicService.GetNetPlayListAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _musicService.CachePlayListAsync(musicList);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                else
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(async () =>
                    {
                        var list = await _musicService.GetNetPlayListAsync();
                        var addedList = new List<MusicModel>();
                        foreach (var item in list)
                        {
                            var target = musicList.First(m => m.Equals(item));
                            if (target == null)
                            {
                                musicList.Add(item);
                                addedList.Add(item);
                            }
                        }
                        _musicService.CachePlayListAsync(musicList);
                        SynchronizationContext.Post((o) =>
                        {
                            foreach (var item in addedList)
                            {
                                MusicList.Add(item);
                            }
                        }, null);

                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }

                foreach (var item in musicList)
                {
                    MusicList.Add(item);
                }
            }
            catch (Exception e)
            {
                App.ShowNotification(e.Message);
                _logger.Error($"Initilize the playlist failed", e);
                return;
            }
        }

        public void ItemClicked(MusicModel model)
        {
            PlayerVM.PlayClicked();
        }
    }
}

