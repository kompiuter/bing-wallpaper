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
                using (var jsonStream = await client.GetStreamAsync("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"))
                {
                    var ser = new DataContractJsonSerializer(typeof(Result));
                    var res = (Result)ser.ReadObject(jsonStream);
                    using (var imgStream = await client.GetStreamAsync(new Uri(baseUri + res.images[0].URL)))
                    {
                        return new BingImage(Image.FromStream(imgStream), res.images[0].Copyright, res.images[0].CopyrightLink);
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
            public string EndDate { get; set; }
            [DataMember(Name = "url")]
            public string URL { get; set; }
            [DataMember(Name = "urlbase")]
            public string URLBase { get; set; }
            [DataMember(Name = "copyright")]
            public string Copyright { get; set; }
            [DataMember(Name = "copyrightlink")]
            public string CopyrightLink { get; set; }
        }
    }

    public class BingImage
    {
        public BingImage(Image img, string copyright, string copyrightLink)
        {
            Img = img;
            Copyright = copyright;
            CopyrightLink = copyrightLink;
        }
        public Image Img { get; set; }
        public string Copyright { get; set; }
        public string CopyrightLink { get; set; }
    }
}
