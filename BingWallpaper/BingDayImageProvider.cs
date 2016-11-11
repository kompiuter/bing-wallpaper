using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.Models
{
    public class BingDayImageProvider : IImageProvider
    {
        private readonly string _cultureInfo;
        public BingDayImageProvider(string cultureInfo = "en-US")
        {
            _cultureInfo = cultureInfo;
        }

        public async Task<Uri> Uri()
        {
            string baseUri = "https://www.bing.com";
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync($"http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt={_cultureInfo}");
                var res = JsonConvert.DeserializeObject<Result>(json);
                return new Uri(baseUri + res.images[0].url);
            }
        }

        private class Result
        {
            public Image[] images { get; set; }
        }
        private class Image
        {
            public string enddate { get; set; }
            public string url { get; set; }
            public string urlbase { get; set; }
        }
    }


}
