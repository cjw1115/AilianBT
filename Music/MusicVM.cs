using AilianBT.Models;
using GalaSoft.MvvmLight;
using MusicBackground;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;

namespace AilianBT.ViewModels
{
    public class MusicVM:ViewModelBase
    {

        public MediaPlayer mediaPlayer = BackgroundMediaPlayer.Current;
        private Services.MusicService musicService;
        public ObservableCollection<MusicModel> Musics { get; set; }
        public MusicVM()
        {
            musicService = new Services.MusicService();
            Musics = new ObservableCollection<MusicModel>();
            InitList();
        }
        public async void InitList()
        {
            
            var list=await musicService.GetNetPlayList();
            foreach (var item in list)
            {
                Musics.Add(item);
            }

            _valueSet = new ValueSet();
            
            _valueSet["action"] = "updatelist";
            _valueSet["playlist"] = JsonHelper.ToJson(list);
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
            
        }
        private ValueSet _valueSet;
        public void Play()
        {
            _valueSet = new ValueSet();
            _valueSet["action"] = "play";
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
        }
        public void Pause()
        {
            _valueSet = new ValueSet();
            _valueSet["action"] = "pause";
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
        }
        public void Next()
        {
            _valueSet = new ValueSet();
            _valueSet["action"] = "next";
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
        }
        public void Previou()
        {
            _valueSet = new ValueSet();
            _valueSet["action"] = "previou";
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
        }
        public void UpdateList()
        {
            _valueSet = new ValueSet();
            _valueSet["action"] = "updatelist";
            _valueSet["playlist"] = Musics;
            BackgroundMediaPlayer.SendMessageToBackground(_valueSet);
        }
    }
}
