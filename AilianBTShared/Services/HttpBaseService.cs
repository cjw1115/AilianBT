using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace AilianBTShared.Services
{
    public class HttpBaseService
    {
        private string user_agent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; RM-1116) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Mobile Safari/537.36 Edge/14.14393";
        private HttpBaseProtocolFilter filter;
        private HttpClient _client;
        public HttpBaseService(bool allowRedirect=true)
        {
            filter = new HttpBaseProtocolFilter();
            filter.AllowAutoRedirect = allowRedirect;
            _client = new HttpClient(filter);
            _client.DefaultRequestHeaders.UserAgent.Clear();
            _client.DefaultRequestHeaders.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue(user_agent));

        }
        public async Task<string> SendRequst(string uri, bool isGb2312 = false, bool ispost=false,IDictionary <string,string> dic=null, string referUri = "", CancellationToken cancellation = new CancellationToken())
        {
            HttpResponseMessage response = null;
            //Encoding gb2312 = Encoding.GetEncoding("gb2312");
            try
            {
                if (!string.IsNullOrEmpty(referUri))
                {
                    _client.DefaultRequestHeaders.Referer = new Uri(referUri);
                }
               
                if (!ispost)
                {
                    response = await _client.GetAsync(new Uri(uri));
                }
                else
                {
                    HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(dic);

                    response = await _client.PostAsync(new Uri(uri), content);
                }

                var buffer = await response.Content.ReadAsBufferAsync();
                byte[] arry = buffer.ToArray();
                var re = Encoding.UTF8.GetString(arry);
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
                    _client.DefaultRequestHeaders.Referer = new Uri(referUri);
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, new Uri(uri));
                
                var re=await _client.SendRequestAsync(request);
                re.EnsureSuccessStatusCode();
                var length = re.Content.Headers.ContentLength;
                 InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                ras.Size = length.Value;
                var inputStream = await _client.GetInputStreamAsync(new Uri(uri));
                RandomAccessStream.CopyAsync(inputStream, ras);
                await Task.Delay(500);
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
