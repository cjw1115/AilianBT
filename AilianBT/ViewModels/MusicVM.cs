using AilianBTShared.Helpers;
using AilianBTShared.Models;
using AilianBTShared.Services;
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
        MediaPlayer player = new MediaPlayer() { AutoPlay = false };
        SystemMediaTransportControls smtc = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
        CoreDispatcher _coreDispatcher;

        public MusicVM(CoreDispatcher coreDispatcher) : this()
        {
            _coreDispatcher = coreDispatcher;
        }
        public MusicVM()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
            Init();
        }
        public async void Init()
        {
            IList<MusicModel> list = null;
            try
            {
                list = await musicService.GetNetPlayList();
                foreach (var item in list)
                {
                    MusicList.Add(item);
                }
            }
            catch(Exception e)
            {
                //App.ShowNotification(e.Message);
                App.ShowNotification("Music 当前不可用");
                return;
            }
          

            ValueSet set = new ValueSet();
            set["action"] = "update";
            set["musiclist"] = JsonHelper.SerializeObject(list);
            BackgroundMediaPlayer.SendMessageToBackground(set);

            IsPlayVisible = true;
            IsPauseVisible = false;
            CanPreviou = true;
            CanNext = true;
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
        private bool _isPlayVisible;
        public bool IsPlayVisible
        {
            get { return _isPlayVisible; }
            set { _isPlayVisible = value; OnPropertyChanged(); }
        }
        private bool _isPauseVisible;
        public bool IsPauseVisible
        {
            get { return _isPauseVisible; }
            set { _isPauseVisible = value; OnPropertyChanged(); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public async void previou_Click()
        {
            ValueSet set = new ValueSet();
            set["action"] = "previou";
            BackgroundMediaPlayer.SendMessageToBackground(set);
        }

        public async void play_Click()
        {
            ValueSet set = new ValueSet();
            set["action"] = "play";
            BackgroundMediaPlayer.SendMessageToBackground(set);
        }

        public async void pause_Click()
        {
            ValueSet set = new ValueSet();
            set["action"] = "pause";
            BackgroundMediaPlayer.SendMessageToBackground(set);
        }

        public async void next_Click()
        {
            ValueSet set = new ValueSet();
            set["action"] = "next";
            BackgroundMediaPlayer.SendMessageToBackground(set);
        }

        public void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            ValueSet valueSet = (ValueSet)e.Data;
            //action:next,play,previou,pause,update
            //play:id
            //update:musiclist
            object actionObject = null;
            valueSet.TryGetValue("action", out actionObject);
            int id = (int)valueSet["id"];
            var model = MusicList.Where(m => m.ID == id).FirstOrDefault();
            if (actionObject != null)
            {
                var action = (string)actionObject;
                switch (action)
                {
                    case "play":
                        _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            IsPlayVisible = false;
                            IsPauseVisible = true;
                            Title = model.Title;
                        });

                        break;
                    case "pause":
                        _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            IsPlayVisible = true;
                            IsPauseVisible = false;
                        });

                        break;
                    case "next":
                        _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            IsPlayVisible = false;
                            IsPauseVisible = true;
                            Title = model.Title;
                        });

                        break;
                    case "previou":
                        _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            IsPlayVisible = false;
                            IsPauseVisible = true;
                            Title = model.Title;
                        });

                        break;
                    case "exception":
                        _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            App.ShowNotification((string)valueSet["des"]);
                        });
                        
                        break;
                    default:
                        break;
                }
            }
        }

        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            MusicModel model = e.ClickedItem as MusicModel;

            ValueSet set = new ValueSet();
            set["action"] = "play";
            set["id"] = model.ID;
            BackgroundMediaPlayer.SendMessageToBackground(set);
        }
    }
}
