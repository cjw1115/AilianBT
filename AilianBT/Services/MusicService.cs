using AilianBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class MusicService
    {
        private readonly string listUri = "http://www.kisssub.org/addon/player/app/scm-music-player/playlist.js?time=";
        private readonly string kisssUbUri = "http://www.kisssub.org/";
        private static HttpBaseService _httpService = new HttpBaseService(true);
        public async Task<IList<MusicModel>> GetNetPlayList()
        {
            List<MusicModel> musicList = new List<MusicModel>();
            var re=await _httpService.SendRequst(listUri+DateTime.Now.Millisecond.ToString());
            if (re != null)
            {
                var startStr = "shuffle(";
                var endStr = "));";
                var startIndex = re.IndexOf(startStr) + startStr.Length;
                var endIndex = re.IndexOf(endStr);

                var jsonString = re.Substring(startIndex, endIndex - startIndex);

                var arry = JsonArray.Parse(jsonString);
                for (int i=0;i<arry.Count;i++)
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
            var newUri=uri.ToString() + "?date=" + DateTime.Now.Millisecond;
            var re = await _httpService.GetUriStream(newUri, referUri: kisssUbUri);
            return re;
        }
    }
}
