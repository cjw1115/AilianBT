using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace AilianBT.DAL.Net
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
        public async Task<string> SendRequst(string uri,bool ispost=false,IDictionary <string,string> dic=null, string referUri = "", CancellationToken cancellation = new CancellationToken())
        {
            HttpResponseMessage response = null;
            Encoding encoding = Encoding.UTF8;
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

                var re=await response.Content.ReadAsStringAsync();
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
        
    }
}
