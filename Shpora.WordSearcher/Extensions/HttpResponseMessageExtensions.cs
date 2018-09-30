using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shpora.WordSearcher.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> DeserializeContent<T>(this HttpResponseMessage message)
        {
            return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync());
        }
    }
}
