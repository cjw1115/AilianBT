using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace AilianBT.Common.Services
{
    public class HttpService
    {
        private const string USER_AGAENT = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; RM-1116) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Mobile Safari/537.36 Edge/14.14393";

        private HttpClientHandler _handler;
        private HttpClient _client;

        public HttpService(bool allowRedirect = true)
        {
            _handler = new HttpClientHandler();
            _handler.UseProxy = false;
            _handler.AllowAutoRedirect = allowRedirect;

            _client = new HttpClient(_handler);
            _client.DefaultRequestHeaders.Add("User-Agent", USER_AGAENT);
            _client.Timeout = TimeSpan.FromMilliseconds(5000);
        }

        public async Task<HttpResponseMessage> SendRequest(string url, HttpMethod method, HttpContent content = null, IDictionary<string, string> appendHeaders = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(method, url);
                request.Content = content;
                if (appendHeaders != null)
                {
                    foreach (var header in appendHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                return await _client.SendAsync(request, completionOption);
            }
            catch
            {
                throw;
            }
            finally
            {
                request?.Dispose();
            }
        }

        public async Task<Stream> SendRequestForStream(string url, HttpMethod method, HttpContent content = null, IDictionary<string, string> appendHeaders = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var response = await SendRequest(url, method, content, appendHeaders, completionOption);
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<IRandomAccessStream> SendRequestForRAS(string url, HttpMethod method, HttpContent content = null, IDictionary<string, string> appendHeaders = null)
        {
            var stream = await SendRequestForStream(url, method, content, appendHeaders);
            return stream.AsRandomAccessStream();
        }

        public async Task<string> SendRequestForString(string url, HttpMethod method, Encoding encoding, HttpContent content = null, IDictionary<string, string> appendHeaders = null)
        {
            using (var stream = await SendRequestForStream(url, method, content, appendHeaders))
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}