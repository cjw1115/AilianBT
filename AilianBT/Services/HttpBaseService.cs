﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace AilianBT.Services
{
    public class HttpBaseService
    {
        private string user_agent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; RM-1116) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Mobile Safari/537.36 Edge/14.14393";
        private HttpClientHandler _handler;
        private HttpClient _client;
        public HttpBaseService(bool allowRedirect=true)
        {
            _handler = new HttpClientHandler();
            _handler.UseProxy = false;
            _handler.AllowAutoRedirect = allowRedirect;
            
            _client = new HttpClient(_handler);
            _client.DefaultRequestHeaders.Add("User-Agent", user_agent);
            //_client.DefaultRequestHeaders.UserAgent.Clear();
            //_client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(user_agent));
            _client.Timeout = TimeSpan.FromMilliseconds(5000);
        }
        public async Task<string> SendRequst(string uri, bool isGb2312 = false, bool ispost=false,IDictionary <string,string> dic=null, string referUri = "", CancellationToken cancellation = new CancellationToken())
        {
            HttpResponseMessage response = null;
            //Encoding gb2312 = Encoding.GetEncoding("gb2312");
            try
            {
                if (!string.IsNullOrEmpty(referUri))
                {
                    _client.DefaultRequestHeaders.Referrer = new Uri(referUri);
                }
               
                if (!ispost)
                {
                    response = await _client.GetAsync(new Uri(uri));
                }
                else
                {

                    FormUrlEncodedContent content = new FormUrlEncodedContent(dic);

                    response = await _client.PostAsync(new Uri(uri), content);
                }

                var buffer = await response.Content.ReadAsByteArrayAsync();
                var re = Encoding.UTF8.GetString(buffer);
                return re;

            }
            catch
            {
                throw;
            }
            finally
            {
                response?.Dispose();
            }
        }
        public async Task<IRandomAccessStream> GetUriStream(string  uri,string referUri = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(referUri))
                {
                    _client.DefaultRequestHeaders.Referrer = new Uri(referUri);
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, new Uri(uri));
                
                var re=await _client.SendAsync(request);
                re.EnsureSuccessStatusCode();
                var length = re.Content.Headers.ContentLength;
                 InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                ras.Size = (ulong)length.Value;
                var stream = await _client.GetStreamAsync(new Uri(uri));
                await RandomAccessStream.CopyAsync(stream.AsInputStream(), ras);
                return ras;
            }
            catch
            {
                throw;
            }
            finally
            {
               
            }
        }
    }
}
