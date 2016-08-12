using AilianBT.Models;
using AilianBT.Services;
using MusicBackground;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
namespace MusicBackground
{
    public sealed class MusicBackground : IBackgroundTask
    {
        private MediaPlayer mediaPlayer;
        private SystemMediaTransportControls smtc;
        private List<MusicModel> playList;
        private MediaSource CurrentSource;
        private int CurrentId { get; set; }

        private MusicService musicService = new MusicService();

        private BackgroundTaskDeferral deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.AutoPlay = false;

            smtc =BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            smtc.IsPauseEnabled = true;
            smtc.IsPreviousEnabled = true;
            smtc.IsNextEnabled = true;
            smtc.IsPlayEnabled = true;
            smtc.ButtonPressed += Smtc_ButtonPressed;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;

            playList = new List<MusicModel>();
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            ValueSet _valueSet = (ValueSet)e.Data;
            object action;
            _valueSet.TryGetValue("action", out action);
            if (action != null)
            {
                switch ((string)action)
                {
                    case "play":
                        object id;
                        _valueSet.TryGetValue("id", out id);
                        if (id != null)
                        {
                            Play((int)id);
                        }
                        else
                        {
                            Play();
                        }
                        break;
                    case "pause": Pause(); break;
                    case "next": Next(); break;
                    case "previou":Previou(); break;
                    case "updatelist":
                        var jsonString = (string)_valueSet["playlist"];
                        var list = JsonHelper.FromJson<IList<MusicModel>>(jsonString);
                        foreach (var item in list)
                        {
                            playList.Add(item);
                        }
                        var first = playList.FirstOrDefault();
                        if (first != null)
                        {
                            CurrentId = first.ID;
                        }
                        break;
                    default:
                        break;
                }
                return;
            }
        }
        public async void Play(int id)
        {
            var item = playList.Where(m => m.ID == id).FirstOrDefault();
            if (item != null)
            {
                await SetMeiaSource(item);
            }
            mediaPlayer.Play();
        }
        public async void Play()
        {
            if (CurrentSource ==null)
            {
                var item = playList.FirstOrDefault();
                if (item != null)
                {
                    await SetMeiaSource(item);
                    mediaPlayer.Play();
                }
            }
            
        }
        
        public void Pause()
        {
            mediaPlayer.Pause();
        }
        public async void Next()
        {
            if (CurrentSource != null)
            {
                int id = (int)CurrentSource.CustomProperties["ID"];
                var item = playList.Where(m => m.ID == id+1).FirstOrDefault();
                if (item != null)
                {
                    await SetMeiaSource(item);
                    mediaPlayer.Play();
                }
                
            }
            
        }
        public async void Previou()
        {
            if (CurrentSource != null)
            {
                int id = (int)CurrentSource.CustomProperties["ID"];
                var item = playList.Where(m => m.ID == id-1).FirstOrDefault();
                if (item != null)
                {
                    await SetMeiaSource(item);
                    mediaPlayer.Play();
                }

            }
        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            BackgroundMediaPlayer.Shutdown();
            deferral.Complete();
        }

        private async Task SetMeiaSource(MusicModel model)
        {
            var ras = await musicService.GetMusicStream(model.Uri);
            CurrentSource = MediaSource.CreateFromStream(ras, "audio/mpeg");
            CurrentSource.CustomProperties["ID"] = model.ID;
            CurrentSource.Reset();
            mediaPlayer.Source = CurrentSource;
        }
    }
}
