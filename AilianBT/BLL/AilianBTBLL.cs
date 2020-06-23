using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AilianBT.Models;
using System.Text.RegularExpressions;
using AilianBT.Exceptions;
using System.Collections.ObjectModel;
using AilianBT.Common.Services;
using System.Text;
using System.Net.Http;
using AngleSharp.Html.Parser;

namespace AilianBT.BLL
{
    public class AilianBTBLL
    {
        public static HttpService _httpService = new HttpService();

        private const string KISSSUB_HOME = "http://m.kisssub.org/";
        private const string KISSSUB_SEARCH = "http://m.kisssub.org/search.php?keyword=";
        private const string KISSSUB_NEW_FAN = "http://m.kisssub.org/addon.php?r=bangumi/plain_format";

        public async Task<IList<AilianResModel>> GetResList(int pageIndex = 1)
        {
            List<AilianResModel> list = null;
            string content = null;
            try
            {
                content = await _httpService.SendRequestForString(KISSSUB_HOME + pageIndex + ".html", HttpMethod.Get, Encoding.UTF8);
            }
            catch(Exception e)
            {
                throw new NetworkException("网络请求异常：" + e.Message);
            }

            try
            {
                var pasrser = new HtmlParser();
                var doc = pasrser.ParseDocument(content);
                var ul = doc.GetElementsByTagName("ul").Skip(1).FirstOrDefault();
                if (ul != null)
                {
                    list = new List<AilianResModel>();
                    var lis = ul.QuerySelectorAll("li");
                    foreach (var item in lis)
                    {
                        AilianResModel model = new AilianResModel();
                        var re = item.Children[0].Children[0].TextContent;
                        model.Title = Regex.Replace(re, @"/\*([\w\W]*)\*/", string.Empty) ?? re;
                        model.Link = KISSSUB_HOME + item.Children[0].Children[0].GetAttribute("href");
                        //
                        var temp = item.Children[1].Children[0].ChildNodes.Last().TextContent.Replace("\n", "").Trim();

                        model.PostTime = temp;

                        temp = item.Children[1].Children[1].ChildNodes.Last().TextContent.Replace("\n", "").Trim();
                        model.Size = temp;
                        model.Author = item.Children[1].Children[2].Children[1].TextContent.Trim();
                        list.Add(model);
                    }
                }
                return list;
            }
            catch(Exception e)
            {
                throw new ResolveException("解析数据异常：" + e.Message);
            }
            
        }
        public async Task<ShowModel> GetDetailHtml(string detailUri)
        {
            ShowModel model = new ShowModel();
            string html = string.Empty;
            string content = null;
            try
            {
                content = await _httpService.SendRequestForString(detailUri, HttpMethod.Get, Encoding.UTF8);
            }
            catch(Exception e)
            {
                throw new NetworkException("网路请求异常：" + e.Message);
            }

            try
            {
                var pasrser = new HtmlParser();
                var doc = pasrser.ParseDocument(content);

                var isLiving = BtDownload.Services.FileService.GetLocalSetting<bool?>("livingmode").Value;

                string torrent_url = null;
                string magnet_url = null;
                if (isLiving)
                {
                    var hasCode = doc.Title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    torrent_url = $"http://v2.uploadbt.com/?r=down&hash={hasCode}";
                    magnet_url = $"magnet:?xt=urn:btih:{hasCode}";
                }
                var ul = doc.GetElementsByClassName("content detail").FirstOrDefault();
                var imgs = ul.QuerySelectorAll("img");
                foreach (var item in imgs)
                {
                    item.SetAttribute("width", "100%");
                }
                model.Summary = "<div style=\"width:100%\">" + ul.OuterHtml + "</div>";

                var file = doc.GetElementsByClassName("content").Skip(1).FirstOrDefault();
                imgs = file.QuerySelectorAll("img");
                foreach (var item in imgs)
                {
                    var link = item.GetAttribute("src");
                    item.SetAttribute("src", KISSSUB_HOME + link);
                }
                model.FileInfo = file.OuterHtml;
                
                if(isLiving)
                {

                    model.BtLink = torrent_url;
                    model.MagnetLink = magnet_url;
                }
                else
                {
                    var div = doc.GetElementsByClassName("down").FirstOrDefault();
                    if (div != null)
                    {
                        var link = KISSSUB_HOME + div.Children[0].GetElementsByTagName("a").FirstOrDefault().GetAttribute("href");
                        model.BtLink = link;
                        link = div.Children[1].GetElementsByTagName("a").FirstOrDefault().GetAttribute("onclick");
                        link = Regex.Match(link, @"magnet[\w\W]*(?=')").ToString();
                        model.MagnetLink = link;
                    }
                }
                return model;
            }
            catch(Exception e)
            {
                throw new ResolveException("解析数据异常：" + e.Message);
            }
            
        }
        public async Task<IList<AilianResModel>> SerachResList(string searchiKey,int pageIndex = 1)
        {
            List<AilianResModel> list = null;
            string content = null;
            try
            {
                content = await _httpService.SendRequestForString(KISSSUB_SEARCH + searchiKey + "&page=" + pageIndex, HttpMethod.Get, Encoding.UTF8);
            }
            catch(Exception e)
            {
                throw new NetworkException("网络请求异常：" + e.Message); 
            }
            try
            {
                var index = content.IndexOf("<div class=\"lists\">");
                content = content.Substring(index);
                var pasrser = new HtmlParser();
                var doc = pasrser.ParseDocument(content);
                var ul = doc.GetElementsByTagName("ul").FirstOrDefault();
                if (ul != null)
                {
                    list = new List<AilianResModel>();
                    var lis = ul.QuerySelectorAll("li");
                    foreach (var item in lis)
                    {
                        AilianResModel model = new AilianResModel();
                        var re = item.Children[0].Children[0].TextContent;
                        model.Title = Regex.Replace(re, @"/\*([\w\W]*)\*/", string.Empty) ?? re;
                        model.Link = KISSSUB_HOME + item.Children[0].Children[0].GetAttribute("href");
                        //
                        var temp = item.Children[1].Children[0].ChildNodes.Last().TextContent.Replace("\n", "").Trim();

                        model.PostTime = temp;

                        temp = item.Children[1].Children[1].ChildNodes.Last().TextContent.Replace("\n", "").Trim();
                        model.Size = temp;
                        model.Author = item.Children[1].Children[2].Children[1].TextContent.Trim();
                        list.Add(model);
                    }
                }
                return list;
            }
            catch(Exception e)
            {
                throw new ResolveException("解析数据异常：" + e.Message);
            }
        }
        public async Task<Dictionary<int, ObservableCollection<Models.NewKeyModel>>> GetNewKeyList()
        {
            Dictionary<int, ObservableCollection<Models.NewKeyModel>> newKeys = new Dictionary<int, ObservableCollection<NewKeyModel>>();

            string content = string.Empty;

            try
            {
                content = await _httpService.SendRequestForString(KISSSUB_NEW_FAN, HttpMethod.Get, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new NetworkException("网络请求异常：" + e.Message);
            }
            var list=content.Split(new string[] {"\r\n"}, StringSplitOptions.None);
            for(int i=0;i<list.Count();i++)
            {
                ObservableCollection<Models.NewKeyModel> newKeysModels = new ObservableCollection<NewKeyModel>(); 
                var keys = list[i].Split(',');
                for (int j = 1; j < keys.Count(); j++)
                {
                    var onekey=keys[j].Split('|');
                    
                    Models.NewKeyModel model = new NewKeyModel { Key = onekey[0], Value = Uri.UnescapeDataString(onekey[1]) };
                    newKeysModels.Add(model);
                }
                newKeys.Add(i, newKeysModels);
            }

            return newKeys;
        }

    }
}
