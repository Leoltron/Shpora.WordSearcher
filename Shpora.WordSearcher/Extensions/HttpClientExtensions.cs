using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shpora.WordSearcher.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, int attempts)
        {
            while (true)
            {
                attempts--;
                try
                {
                    return await client.GetAsync(requestUri);
                }
                catch (Exception e)
                {
                    if (attempts == 0)
                        throw;
                    Logger.Log.Error($"An error occured during GET request sending, {attempts} attempt(s) left", e);
                }
            }
        }

        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, HttpContent content, int attempts)
        {
            while (true)
            {
                attempts--;
                try
                {
                    return await client.PostAsync(requestUri, content);
                }
                catch (Exception e)
                {
                    if (attempts == 0)
                        throw;
                    Logger.Log.Error($"An error occured during POST request sending, {attempts} attempt(s) left", e);
                }
            }
        }
    }
}
