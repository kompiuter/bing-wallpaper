using System;
using System.Drawing;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace BingWallpaper
{
    public class BingImageProvider
    {
        public async Task<BingImage> GetImage()
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
                        return new BingImage(Image.FromStream(imgStream), res.images[0].copyright);
                    }
                }
            }
        }

        [DataContract]
        private class Result
        {
            [DataMember(Name = "images")]
            public ResultImage[] images { get; set; }
        }

        [DataContract]
        private class ResultImage
        {
            [DataMember(Name = "enddate")]
            public string enddate { get; set; }
            [DataMember(Name = "url")]
            public string url { get; set; }
            [DataMember(Name = "urlbase")]
            public string urlbase { get; set; }
            [DataMember(Name = "copyright")]
            public string copyright { get; set; }
        }
    }

    public class BingImage
    {
        public BingImage(Image img, string copyright)
        {
            Img = img;
            Copyright = copyright;
        }
        public Image Img { get; set; }
        public string Copyright { get; set; }
    }
}
