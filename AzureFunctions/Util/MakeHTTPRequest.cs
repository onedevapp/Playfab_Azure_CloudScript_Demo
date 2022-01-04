using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SwipeWire.PlayfabCloudScript.Util
{
    public class MakeHTTPRequest
    {
        private static async Task<string> MakePostURI(string url, string payload, Dictionary<string, string> _headers, string contentType)
        {            
            var requestContent = new StringContent(payload, Encoding.UTF8, string.IsNullOrEmpty(contentType) ? "application/json" : contentType);
            Uri _uri = new Uri(url);

            var result = string.Empty;
            using (var _client = new HttpClient())
            {
                foreach(KeyValuePair<string, string> entry in _headers)
                {
                    _client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }

                var response = await _client.PostAsync(_uri, requestContent);
                
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }

            return result;
        }

        public static async Task<string> MakeGetURI(string url)
        {
            Uri _uri = new Uri(url);

            var response = string.Empty;
            using (var _client = new HttpClient())
            {
                var result = await _client.GetAsync(_uri);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }
            }
            return response;
        }

    }
}