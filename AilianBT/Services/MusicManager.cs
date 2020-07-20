using AilianBT.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.WebUI;

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
            _mediaPlaybackList.ItemOpened += _mediaPlaybackListItemOpened;
            _mediaPlaybackList.ItemFailed += _mediaPlaybackListItemFailed;
            _mediaPlaybackList.AutoRepeatEnabled = false;
            _mediaPlaybackList.MaxPlayedItemsToKeepOpen = 3;

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Source = _mediaPlaybackList;
            _mediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            _mediaPlayer.MediaEnded += _mediaPlayerMediaEnded;
        }

        private void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            PositionChanged?.Invoke(_mediaPlayer.PlaybackSession.Position, _mediaPlayer.PlaybackSession.NaturalDuration);
        }

        public event Action<MusicModel> MediaLoaded;
        public event Action<MusicModel> MediaEnd;
        public event Action<MusicModel> MediaFailed;
        public event Action<MusicModel> MediaCached;
        public event Action<TimeSpan, TimeSpan> PositionChanged;
        

        #region MediaPlaybackList callbacks
        private void _mediaPlaybackListItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {
            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonSerializer.Deserialize<MusicModel>(modelJson);
            _logger.Error($"Open Successfully: \n\t{modelJson}");
            MediaLoaded?.Invoke(model);
        }

        private void _mediaPlayerMediaEnded(MediaPlayer sender, object args)
        {
            MediaEnd?.Invoke(null);
        }

        private void _mediaPlaybackListItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            MusicModel model = null;
            if(args.Item.Source.CustomProperties.TryGetValue("model", out object value))
            {
                var modelJson = value as string;
                model = JsonSerializer.Deserialize<MusicModel>(modelJson);
                _logger.Error($"Play music failed: \n\t{modelJson}\n\t{args.Error.ErrorCode}", args.Error.ExtendedError);
            }
            MediaFailed?.Invoke(model);
        }
        #endregion

        public async Task<MediaPlaybackItem> Add(MusicModel model)
        {
            var indexInPlaybackList = _getIndex(model);

            // Prepare audio steam
            var source = await musicService.GetCahchedMusicAsync(model);
            if (source == null)
            {
                try
                {
                    source = await musicService.RequestMusic(model);
                }
                catch(Exception e)
                {
                    _logger.Error($"Get music stream failed", e);
                }
            }

            var playbackItem = _createMediaPlaybackItem(source, model);
            if (indexInPlaybackList != null)
            {
                _mediaPlaybackList.Items[indexInPlaybackList.Value] = playbackItem;
            }
            else
            {
                _mediaPlaybackList.Items.Add(playbackItem);
            }
            return playbackItem;
        }

        private MediaPlaybackItem _createMediaPlaybackItem(MediaSource source, MusicModel model)
        {
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

        public async Task Play(MusicModel model)
        {
            if (_isPaused)
            {
                _mediaPlayer.Play();
            }
            else
            {
                await _play(model);
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

        public async Task Previous(MusicModel model)
        {
            await _play(model);
        }

        public async Task Next(MusicModel model)
        {
            await _play(model);
        }

        public async Task Seek(TimeSpan position)
        {
            _mediaPlayer.PlaybackSession.Position = position;
        }
        private async Task _play(MusicModel model)
        {
            try
            {
                var playbackItem = await _cacheMusic(model);
                var playbackItemIndex = _mediaPlaybackList.Items.IndexOf(playbackItem);
                _mediaPlaybackList.MoveTo((uint)playbackItemIndex);
                _mediaPlayer.Play();
            }
            catch
            {
                MediaFailed?.Invoke(model);
            }
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

        private async Task<MediaPlaybackItem> _cacheMusic(MusicModel model)
        {
            if (!CachedMusicList.Any(m => m.Title == model.Title))
            {
                CachedMusicList.Add(model);
            }
            return await Add(model);
        }
    }
}
