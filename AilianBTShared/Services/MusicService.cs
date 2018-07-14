using AilianBTShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;

namespace AilianBTShared.Services
{
    public class MusicService
    {
        private readonly string listUri = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js";
        private readonly string kisssUbUri = "http://www.kisssub.org/";
        private static HttpBaseService _httpService = new HttpBaseService(true);
        public async Task<IList<MusicModel>> GetNetPlayList()
        {
            List<MusicModel> musicList = new List<MusicModel>();
            var re = await _httpService.SendRequst(listUri);
            if (re != null)
            {
                var jsonString = Regex.Match(re, @"(?<=shuffle\()([\w\W]*)(?=\)\);SCM)").ToString();

                var arry = JsonArray.Parse(jsonString);
                for (int i = 0; i < arry.Count; i++)
                {
                    MusicModel model = new MusicModel();
                    model.ID = i;
                    model.Title = arry[i].GetObject()["title"].GetString();
                    var uriString = arry[i].GetObject()["url"].GetString();
                    model.Uri = new Uri(uriString);
                    musicList.Add(model);
                }
            }
            return musicList;
        }
        public async Task<IRandomAccessStream> GetMusicStream(Uri uri)
        {
            var re = await _httpService.GetUriStream(uri.ToString(),referUri: kisssUbUri);
            return re;
        }
    }
}
