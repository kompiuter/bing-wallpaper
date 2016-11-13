using System;
using System.Drawing;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace BingWallpaper.Models
{
    public class BingDayImageProvider : IImageProvider
    {
        public async Task<Image> GetImage()
        {
            string baseUri = "https://www.bing.com";
            using (var client = new HttpClient())
            {
                using (var jsonStream = await client.GetStreamAsync($"http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"))
                {
                    var ser = new DataContractJsonSerializer(typeof(Result));
                    var res = (Result)ser.ReadObject(jsonStream);
                    using (var imgStream = await client.GetStreamAsync(new Uri(baseUri + res.images[0].url)))
                    {
                        return Image.FromStream(imgStream);
                    }
                }
            }
        }

        [DataContract]
        private class Result
        {
            [DataMember(Name = "images")]
            public BingImage[] images { get; set; }
        }
        [DataContract]
        private class BingImage
        {
            [DataMember(Name = "enddate")]
            public string enddate { get; set; }
            [DataMember(Name = "url")]
            public string url { get; set; }
            [DataMember(Name = "urlbase")]
            public string urlbase { get; set; }
        }
    }


}
