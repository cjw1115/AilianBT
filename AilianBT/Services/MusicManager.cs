using AilianBT.Models;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace AilianBT.Services
{
    public class MusicManager
    {
        private MediaPlayer _mediaPlayer;
        private MediaPlaybackList _mediaPlaybackList;
        private MusicService musicService = SimpleIoc.Default.GetInstance<MusicService>();
        private LogService _logger = SimpleIoc.Default.GetInstance<LogService>();

        public List<MusicModel> CachedMusicList { get; private set; } = new List<MusicModel>();

        public MusicManager()
        {
            _mediaPlaybackList = new MediaPlaybackList();
            _mediaPlaybackList.CurrentItemChanged += _mediaPlaybackListCurrentItemChanged;
            _mediaPlaybackList.ItemOpened += _mediaPlaybackListItemOpened;
            _mediaPlaybackList.ItemFailed += _mediaPlaybackListItemFailed;
            _mediaPlaybackList.AutoRepeatEnabled = false;
            _mediaPlaybackList.MaxPlayedItemsToKeepOpen = 3;

            _mediaPlaybackList.Items.Add(_createPlaceholderItem());
            _mediaPlaybackList.Items.Add(_createPlaceholderItem());
            _mediaPlaybackList.Items.Add(_createPlaceholderItem());

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Source = _mediaPlaybackList;
        }

        public event Action<MusicModel> MediaLoaded;
        public event Action<MusicModel> MediaFailed;
        public event Action<MusicModel, MusicModel> MediaChanged;
        public event Action<MusicModel> MediaLoading;
        public event Action<MusicModel> MediaCached;
        public event Action MediaEnd;

        #region MediaPlaybackList callbacks
        private void _mediaPlaybackListCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if(args.Reason== MediaPlaybackItemChangedReason.EndOfStream)
            {
                _mediaPlayer.Pause();
                MediaEnd?.Invoke();
                return;
            }

            MusicModel newModel = null;
            MusicModel oldModel= null;
            if(args.NewItem!=null)
            {
                var newModelJson = args.NewItem.Source.CustomProperties["model"] as string;
                newModel = JsonSerializer.Deserialize<MusicModel>(newModelJson);
            }
            if (args.OldItem != null)
            {
                var oldModelJson = args.OldItem.Source.CustomProperties["model"] as string;
                oldModel = JsonSerializer.Deserialize<MusicModel>(oldModelJson);
            }
            MediaChanged?.Invoke(newModel, oldModel);
        }

        private void _mediaPlaybackListItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {
            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonSerializer.Deserialize<MusicModel>(modelJson);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Successfully:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
            MediaLoaded?.Invoke(model);
        }

        private void _mediaPlaybackListItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            if(args.Item.Source.CustomProperties.TryGetValue("model", out object value))
            {
                var modelJson = value as string;
                var model = JsonSerializer.Deserialize<MusicModel>(modelJson);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Open Failed:");
                System.Diagnostics.Debug.WriteLine(modelJson);
#endif
                MediaFailed?.Invoke(model);
            }
        }
        #endregion

        public async Task Add(MusicModel model)
        {
            var indexInPlaybackList = _getIndex(model);

            // Prepare audio steam
            var stream = await musicService.GetCahchedMusicAsync(model);
            if (stream == null)
            {
                MediaLoading?.Invoke(model);
                try
                {
                    stream = await musicService.GetMusicStream(model.Uri);
                }
                catch(Exception e)
                {
                    MediaFailed?.Invoke(model);
                    _logger.Error($"Get music stream failed", e);
                }

                //Cache media and send a notification when cache successfully.
                musicService.CahcheMusic(stream, model);
                MediaCached?.Invoke(model);
            }

            if (indexInPlaybackList != null)
            {
                _mediaPlaybackList.Items[indexInPlaybackList.Value] = _createMediaPlaybackItem(stream, model);
            }
            else
            {
                _mediaPlaybackList.Items.Add(_createMediaPlaybackItem(stream, model));
            }
        }

        private MediaPlaybackItem _createMediaPlaybackItem(Stream sourceStream, MusicModel model)
        {
            //Create Media source
            var source = MediaSource.CreateFromStream(sourceStream.AsRandomAccessStream(), "audio/mpeg");
            source.CustomProperties["model"] = JsonSerializer.Serialize(model);
            var item = new MediaPlaybackItem(source);
            //Set STMC
            var props = item.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = model.Title;
            item.ApplyDisplayProperties(props);
            return item;
        }

        private bool _isPaused;

        public void Play(MusicModel model)
        {
            if (_isPaused)
            {
                _mediaPlayer.Play();
            }
            else
            {
                var index = _getIndex(model);
                if (index == null)
                {
                    //Media is in downloading...
                    MediaLoading?.Invoke(model);
                    return;
                }
                _mediaPlaybackList.MoveTo((uint)index.Value);
                _mediaPlayer.Play();
            }
        }

        public void Pause(bool stop = false)
        {
            if(stop!=true)
            {
                _isPaused = true;
            }
            _mediaPlayer.Pause();
        }

        public void Previous(MusicModel model)
        {
            var index = _getIndex(model);
            if (index == null)
            {
                //Media is in downloading...
                MediaLoading?.Invoke(model);
                return;
            }
            _mediaPlaybackList.MoveTo((uint)index.Value);
            _mediaPlayer.Play();
        }

        public void Next(MusicModel model)
        {
            var index = _getIndex(model);
            if(index==null)
            {
                //Media is in downloading...
                MediaLoading?.Invoke(model);
                return;
            }
            _mediaPlaybackList.MoveTo((uint)index.Value);
            _mediaPlayer.Play();
        }

        private int? _getIndex(MusicModel model)
        {
            int? index = null;
            for (int i = 0; i < _mediaPlaybackList.Items.Count; i++)
            {
                _mediaPlaybackList.Items[i].Source.CustomProperties.TryGetValue("model", out object value);
                if (value == null)
                    continue;
                var item = JsonSerializer.Deserialize<MusicModel>(value as string);
                if (model.Equals(item))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private MediaPlaybackItem _createPlaceholderItem()
        {
            return new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri("cq://cq")));
        }
    }
}
