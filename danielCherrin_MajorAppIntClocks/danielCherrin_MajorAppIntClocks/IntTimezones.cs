using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;

namespace danielCherrin_MajorAppIntClocks
{
    [Serializable]
    public static class IntTimezones
    {
        public static async Task<List<string>> GetTimezones()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://worldtimeapi.org/api/timezone");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString = responseString.Trim('[');
            responseString = responseString.Trim(']');
            return new List<string>(responseString.Split(','));
        }
    }
}
