using AilianBT.Common.Models;
using AilianBT.Common.Services;
using AilianBT.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
        private List<MusicModel> _cachedMusicList;
        public List<MusicModel> CachedMusicList => _cachedMusicList;
        private MusicService musicService = new MusicService();

        public MusicManager()
        {
            _mediaPlaybackList = new MediaPlaybackList();
            _mediaPlaybackList.CurrentItemChanged += _mediaPlaybackList_CurrentItemChanged;
            _mediaPlaybackList.ItemOpened += _mediaPlaybackList_ItemOpened;
            _mediaPlaybackList.ItemFailed += _mediaPlaybackList_ItemFailed;
            _mediaPlaybackList.AutoRepeatEnabled = false;
            


            _mediaPlaybackList.MaxPlayedItemsToKeepOpen = 3;
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.Source = _mediaPlaybackList;

            InitSource();

            _cachedMusicList = new List<MusicModel>(3);
            _cachedMusicList.Add(new MusicModel());
            _cachedMusicList.Add(new MusicModel());
            _cachedMusicList.Add(new MusicModel());

        }
        private void InitSource()
        {
            _mediaPlaybackList.Items.Add(GetPlaceholderItem());
            _mediaPlaybackList.Items.Add(GetPlaceholderItem());
            _mediaPlaybackList.Items.Add(GetPlaceholderItem());
        }
        public MediaPlaybackItem GetPlaceholderItem()
        {
            MediaPlaybackItem item1 = new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri("cq://cq")));
            return item1;
        }
        public event Action<MusicModel> MediaLoaded;
        public event Action<MusicModel> MediaFailed;
        public event Action<MusicModel, MusicModel> MediaChanged;
        public event Action<MusicModel> MediaLoading;
        public event Action<MusicModel> MediaCached;
        public event Action MediaEnd;

        private void _mediaPlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            args.Item.Source.CustomProperties.TryGetValue("model",out object value);
            if (value == null)
                return;
            var modelJson = value as string;
            var model = JsonSerializer.Deserialize<MusicModel>(modelJson);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Failed:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
            MediaFailed?.Invoke(model);
        }

        private void _mediaPlaybackList_ItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {

            var modelJson = args.Item.Source.CustomProperties["model"] as string;
            var model = JsonSerializer.Deserialize<MusicModel>(modelJson);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Open Successfully:");
            System.Diagnostics.Debug.WriteLine(modelJson);
#endif
            MediaLoaded?.Invoke(model);
        }

        private void _mediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
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
        
        public async Task Add(MusicModel model,int index)
        {
            for (int i = 0; i < _mediaPlaybackList.Items.Count; i++)
            {
                var customProperties = _mediaPlaybackList.Items[i].Source.CustomProperties;
                if(customProperties.ContainsKey("model"))
                {
                    var cacheItem = JsonSerializer.Deserialize<MusicModel>(customProperties["model"] as string);
                    if(cacheItem.Equals(model))
                    {
                        if (i != index)
                        {
                            var temp=_mediaPlaybackList.Items[i];
                            _mediaPlaybackList.Items.RemoveAt(i);
                            _mediaPlaybackList.Items.Insert(i, GetPlaceholderItem());
                            _mediaPlaybackList.Items[index] = temp;
                        }
                        else
                        {

                        }
                        return;
                    }
                }
            }

            //GetLocal
            IRandomAccessStream ras = null;
            ras = await musicService.GetCahchedMusicAsync(model);
            if(ras==null)
            {
                MediaLoading?.Invoke(model);

                try
                {
                    ras = await musicService.GetMusicStream(model.Uri);
                }
                catch
                {
                    throw new OpenStreamFailedException();
                }
                //Cache media and send a notification when cache successfully.
                var cachedResult = await musicService.CahcheMusicAsync(ras, model);
                if (cachedResult == true)
                {
                    MediaCached?.Invoke(model);
                }
            }
            else
            {

            }
            
            //
            if (!model.Equals(_cachedMusicList[index]))
            {
                ras.Dispose();
                return;
            }
            
            //Create Media source
            var source = MediaSource.CreateFromStream(ras, "audio/mpeg");
            var modelJson = JsonSerializer.Serialize<MusicModel>(model);
            source.CustomProperties["model"] = modelJson;
            MediaPlaybackItem item = new MediaPlaybackItem(source);

            //Set STMC
            var props= item.GetDisplayProperties();
            props.Type = MediaPlaybackType.Music;
            props.MusicProperties.Title = model.Title;
            item.ApplyDisplayProperties(props);

            _mediaPlaybackList.Items[index] = item;
        }

        private const string CachedMusic = "CachedMusic";
        
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
            if(_isPaused==true)
            {
                _mediaPlayer.Play();
            }
            else
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
        }
        private bool _isPaused;
        public void Pause(bool stop = false)
        {
            if(stop!=true)
            {
                _isPaused = true;
            }
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
                _mediaPlaybackList.Items[i].Source.CustomProperties.TryGetValue("model", out object value);
                if (value == null)
                    continue;
                var item = JsonSerializer.Deserialize<MusicModel>(value as string);
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
