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
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Source = _mediaPlaybackList;


        }

        public event Action<MusicModel> MediaLoaded;
        public event Action<MusicModel> MediaFailed;
        public event Action<MusicModel, MusicModel> MediaChanged;
        public event Action<MusicModel> MediaLoading;

        private void _mediaPlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonHelper.DerializeObjec<MusicModel>(modelJson);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Failed:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
            MediaFailed?.Invoke(model);
        }

        private void _mediaPlaybackList_ItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {

            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonHelper.DerializeObjec<MusicModel>(modelJson);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Successfully:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
            MediaLoaded?.Invoke(model);
        }

        private void _mediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            MusicModel newModel = null;
            MusicModel oldModel= null;
            if(args.NewItem!=null)
            {
                var newModelJson = args.NewItem.Source.CustomProperties["model"] as string;
                newModel = JsonHelper.DerializeObjec<MusicModel>(newModelJson);
            }
            if (args.OldItem != null)
            {
                var oldModelJson = args.OldItem.Source.CustomProperties["model"] as string;
                oldModel = JsonHelper.DerializeObjec<MusicModel>(oldModelJson);
            }
            MediaChanged?.Invoke(newModel, oldModel);
        }
        
        public async Task Add(MusicModel model,int? index=null)
        {
            IRandomAccessStream ras = null;
            try
            {
                ras = await musicService.GetMusicStream(model.Uri);
            }
            catch
            {
                throw new OpenStreamFailedException();
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
        public void Previous(MusicModel model)
        {
            var index = GetIndex(model);
            if (index == null)
            {
                //Media is in downloading...
                MediaLoading?.Invoke(model);
                return;
            }
            _mediaPlaybackList.MoveTo(index.Value);
            _mediaPlayer.Play();
        }
        public void Play(MusicModel model)
        {
            var index = GetIndex(model);
            if (index == null)
            {
                //Media is in downloading...
                MediaLoading?.Invoke(model);
                return;
            }
            _mediaPlaybackList.MoveTo(index.Value);
            _mediaPlayer.Play();
        }
        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        public void Next(MusicModel model)
        {
            var index = GetIndex(model);
            if(index==null)
            {
                //Media is in downloading...
                MediaLoading?.Invoke(model);
                return;
            }
            _mediaPlaybackList.MoveTo(index.Value);
            _mediaPlayer.Play();
        }

        private uint? GetIndex(MusicModel model)
        {
            uint? index = null;
            for (int i = 0; i < _mediaPlaybackList.Items.Count; i++)
            {
                var item = JsonHelper.DerializeObjec<MusicModel>(_mediaPlaybackList.Items[i].Source.CustomProperties["model"] as string);
                if (model.Equals(item))
                {
                    index = (uint)i;
                    break;
                }
            }
            return index;
        }
    }
}
