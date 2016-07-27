using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AilianBT.Models;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Windows.Web.Http;
using System.Text.RegularExpressions;
namespace AilianBT.BLL
{
    public class AilianBTBLL
    {
        public static DAL.Net.HttpBaseService _httpService = new DAL.Net.HttpBaseService();

        private readonly string 首页 = "http://m.kisssub.org/";
        private readonly string 合集 = "http://m.kisssub.org/complete-1.html ";
        private readonly string 搜索= "http://m.kisssub.org/search.php?keyword=";

        public async Task<IList<AilianResModel>> GetResList(int pageIndex = 1)
        {
            List<AilianResModel> list = null;
            try
            {
                var content = await _httpService.SendRequst(首页+ pageIndex + ".html");
                var pasrser = new HtmlParser();
                var doc = pasrser.Parse(content);
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
                        model.Link = 首页 + item.Children[0].Children[0].GetAttribute("href");
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
            catch
            {
                throw;
            }
            
        }
        public async Task<ShowModel> GetDetailHtml(string detailUri)
        {
            ShowModel model = new ShowModel();
            string html = string.Empty;
            var content = await _httpService.SendRequst(detailUri);
            var pasrser = new HtmlParser();
            var doc = pasrser.Parse(content);


            //var links=doc.GetElementsByTagName("link").Where(m => m.HasAttribute("rel") && m.GetAttribute("rel") == "stylesheet");
            //foreach(var item in links)
            //{
            //    var href = item.GetAttribute("href");
            //    item.SetAttribute("href", 首页 + href);
            //    html += item.OuterHtml + "<br/>";
            //}
            
            var ul = doc.GetElementsByClassName("content detail").FirstOrDefault();
            var imgs=ul.QuerySelectorAll("img");
            foreach (var item in imgs)
            {
                item.SetAttribute("width", "100%");
            }
            model.Summary = "<div style=\"width:100%\">" + ul.OuterHtml+"</div>";

            var file= doc.GetElementsByClassName("content").Skip(1).FirstOrDefault();
            imgs=file.QuerySelectorAll("img");
            foreach (var item in imgs)
            {
                var link= item.GetAttribute("src");
                item.SetAttribute("src", 首页+ link);
            }
            model.FileInfo = file.OuterHtml;

            var div=doc.GetElementsByClassName("down").FirstOrDefault();
            if (div != null)
            {
                var link=首页+div.Children[0].GetElementsByTagName("a").FirstOrDefault().GetAttribute("href");
                model.BtLink = link;
                link= div.Children[1].GetElementsByTagName("a").FirstOrDefault().GetAttribute("onclick");
                link=Regex.Match(link, @"magnet[\w\W]*(?=')").ToString();
                model.MagnetLink = link;
            }
            return model;
        }
        public async Task<IList<AilianResModel>> SerachResList(string searchiKey,int pageIndex = 1)
        {
            List<AilianResModel> list = null;
            try
            {
                var content = await _httpService.SendRequst(搜索 + searchiKey +"&page="+ pageIndex);
                var index = content.IndexOf("<div class=\"lists\">");
                content=content.Substring(index);
                var pasrser = new HtmlParser();
                var doc = pasrser.Parse(content);
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
                        model.Link = 首页 + item.Children[0].Children[0].GetAttribute("href");
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
            catch
            {
                throw;
            }
        }

    }
}
