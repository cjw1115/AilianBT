using AilianBT.Common.Models;
using AilianBT.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace MusicBackground
{
    public sealed class MusicBackground : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private MediaPlayer mediaPlaer;
        SystemMediaTransportControls smtc = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
        private int currentID = -1;
        private List<MusicModel> musicList = new List<MusicModel>(0);
        private MusicService musicService = new MusicService();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            mediaPlaer = BackgroundMediaPlayer.Current;
            mediaPlaer.AutoPlay = false;
            mediaPlaer.MediaEnded += (o, args) => { positon = TimeSpan.Zero; currentID++; play(); };

            smtc.IsPreviousEnabled = true;
            smtc.IsNextEnabled = true;
            smtc.IsPauseEnabled = true;
            smtc.IsPlayEnabled = true;
            smtc.ButtonPressed += Smtc_ButtonPressed;
            smtc.DisplayUpdater.Type = MediaPlaybackType.Music;

            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    play(isFromForeground:false);
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    pause(isFromForeground: false);
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    break;
                case SystemMediaTransportControlsButton.Record:
                    break;
                case SystemMediaTransportControlsButton.FastForward:
                    break;
                case SystemMediaTransportControlsButton.Rewind:
                    break;
                case SystemMediaTransportControlsButton.Next:
                    next(isFromForeground: false);
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    previou(isFromForeground: false);
                    break;
                case SystemMediaTransportControlsButton.ChannelUp:
                    break;
                case SystemMediaTransportControlsButton.ChannelDown:
                    break;
                default:
                    break;
            }
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            ValueSet valueSet = (ValueSet)e.Data;
            //action:next,play,previou,pause,update
            //play:id
            //update:musiclist
            object actionObject = null;
            valueSet.TryGetValue("action",out actionObject);
            if (actionObject != null)
            {
                var action = (string)actionObject;
                switch (action)
                {
                    case "play":
                        object id = null;
                        valueSet.TryGetValue("id", out id);
                        if (id == null)
                            play();
                        else
                        {
                            play((int)id);
                        }
                        break;
                    case "pause":pause(); break;
                    case "next":next(); break;
                    case "previou":previou(); break;
                    case "update":
                        valueSet.TryGetValue("musiclist", out object listObject);
                        if (listObject != null)
                        {
                            var list = JsonSerializer.Deserialize<List<MusicModel>>((string)listObject);
                            musicList.Clear();
                            musicList.AddRange(list);
                            currentID = musicList.FirstOrDefault().ID;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            deferral.Complete();
        }

        private async void previou(bool isFromForeground = true)
        {
            currentID--;
            if (currentID < 0 || currentID >= musicList.Count)
            {
                currentID = musicList.Count - 1;
            }
            var model = musicList[currentID];
            IRandomAccessStream ras;
            try
            {
                ras = await musicService.GetMusicStream(model.Uri);
            }
            catch
            {
                SendMessageToForground("exception", currentID,"网络错误");
                return;
            }
            var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
            mediaPlaer.Source = source;
            mediaPlaer.Play();
            
            displayUpdate(model);
          
            SendMessageToForground("previou", currentID);
        }

        private async void play(bool isFromForeground = true)
        {
            if(currentID<0|| currentID >= musicList.Count)
            {
                currentID = 0;
            }
            if (positon == TimeSpan.Zero)
            {
                var model = musicList[currentID];
                IRandomAccessStream ras;
                try
                {
                    ras = await musicService.GetMusicStream(model.Uri);
                }
                catch
                {
                    SendMessageToForground("exception", currentID, "网络错误");
                    return;
                }
                var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
                
                mediaPlaer.Source = source;
                
                displayUpdate(model);
            }
            mediaPlaer.PlaybackSession.Position = positon;
            mediaPlaer.Play();
           
            SendMessageToForground("play", currentID);

        }
        private async void play(int id, bool isFromForeground = true)
        {
            if (id < 0 || id >= musicList.Count)
                return;
            currentID = id;
            
            var model = musicList[currentID];
            IRandomAccessStream ras;
            try
            {
                ras = await musicService.GetMusicStream(model.Uri);
            }
            catch
            {
                SendMessageToForground("exception", currentID, "网络错误");
                return;
            }
            
            var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
            mediaPlaer.Source = source;
            
            mediaPlaer.Play();
            displayUpdate(model);
            //if (isFromForeground)
            //{
            //    SendMessageToForground("play", currentID);
            //}
            SendMessageToForground("play", currentID);
        }

        private TimeSpan positon;
        private  void pause(bool isFromForeground = true)
        {
            positon = mediaPlaer.PlaybackSession.Position;
            mediaPlaer.Pause();
            //if (isFromForeground)
            //{
            //    SendMessageToForground("pause", currentID);
            //}
            SendMessageToForground("pause", currentID);
        }

        private async void next(bool isFromForeground=true)
        {
            currentID++;
            if (currentID < 0 || currentID >= musicList.Count)
            {
                currentID = 0;
            }
            var model = musicList[currentID];
            IRandomAccessStream ras;
            try
            {
                ras = await musicService.GetMusicStream(model.Uri);
            }
            catch
            {
                SendMessageToForground("exception", currentID, "网络错误");
                return;
            }
            var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
            mediaPlaer.Source = source;
            mediaPlaer.Play();
            
            displayUpdate(model);
            //if (isFromForeground)
            //{
            //    SendMessageToForground("next", currentID);
            //}
            SendMessageToForground("next", currentID);
        }

        private void displayUpdate(MusicModel model)
        {
            smtc.DisplayUpdater.MusicProperties.Title = model.Title;
            smtc.DisplayUpdater.Update();
        }
        private void SendMessageToForground(string action,int currentId,string des=null)
        {
            ValueSet set = new ValueSet();
            set["action"] = action;
            set["id"] = currentId;
            if(action== "exception")
            {
                set["des"] = des;
            }
            BackgroundMediaPlayer.SendMessageToForeground(set);
        }
    }
}

