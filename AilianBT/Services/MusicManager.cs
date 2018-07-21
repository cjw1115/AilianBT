using AilianBT.Exceptions;
using AilianBT.Helpers;
using AilianBT.Models;
using AilianBT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class MusicManager
    {
        private MediaPlayer _mediaPlayer;
        private MediaPlaybackList _mediaPlaybackList;

        private MusicService musicService = new MusicService();

        public MusicManager()
        {
            _mediaPlaybackList = new MediaPlaybackList();
            _mediaPlaybackList.CurrentItemChanged += _mediaPlaybackList_CurrentItemChanged;
            _mediaPlaybackList.ItemOpened += _mediaPlaybackList_ItemOpened;
            _mediaPlaybackList.ItemFailed += _mediaPlaybackList_ItemFailed;

            _mediaPlaybackList.MaxPlayedItemsToKeepOpen = 3;
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Source = _mediaPlaybackList;
        }

        private void _mediaPlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonHelper.DerializeObjec<MusicModel>(modelJson);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Failed:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
        }

        private void _mediaPlaybackList_ItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {

            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonHelper.DerializeObjec<MusicModel>(modelJson);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Successfully:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
        }

        private void _mediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            
        }
        
        public async void Add(MusicModel model,int? index=null)
        {
            IRandomAccessStream ras = null;
            try
            {
                ras = await musicService.GetMusicStream(model.Uri);
            }
            catch
            {
                throw new OpenStreamFailedException();
                return;
            }

            var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
            MediaPlaybackItem item = new MediaPlaybackItem(source);
            var modelJson=JsonHelper.SerializeObject<MusicModel>(model);
            source.CustomProperties["model"] = modelJson;

            var props= item.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = model.Title;
            item.ApplyDisplayProperties(props);

            if (index != null)
            {
                _mediaPlaybackList.Items.Insert(index.Value, item);
                //_musicList.Insert(index.Value, model);
            }
            else
            {
                _mediaPlaybackList.Items.Add(item);
                //_musicList.Add(model);
            }
            
        }
        public void Clear(int? index=null)
        {
            if(index==null)
            {
                foreach (var item in _mediaPlaybackList.Items)
                {
                    item.Source.Dispose();
                }
                _mediaPlaybackList.Items.Clear();
            }
            else
            {
                _mediaPlaybackList.Items.ToArray()[index.Value].Source.Dispose();
                _mediaPlaybackList.Items.RemoveAt(index.Value);
            }
        }
        public void Previous()
        {
            _mediaPlaybackList.MovePrevious();
            _mediaPlayer.Play();
        }
        public void Play()
        {
            _mediaPlayer.Play();
        }
        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        public void Next()
        {
            _mediaPlaybackList.MoveNext();
            _mediaPlayer.Play();
        }
    }
}
