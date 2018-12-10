namespace Fanex.Nap.Client.AspNetCore.Tests
{
    using System.Net.Http;
    using Newtonsoft.Json;
    
    public static class CommonUtils
    {
        public static T GetFromResponse<T>(HttpResponseMessage response)
        {
            var content =  response.Content.ReadAsStringAsync().Result;
            
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}