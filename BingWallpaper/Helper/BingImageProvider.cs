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
        public async Task<HistoryImage> GetLatestImage()
        {
            //var story = await GetCoverstory();

            using (var client = new HttpClient())
            {
                using (var jsonStream = await client.GetStreamAsync("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"))
                {
                    var ser = new DataContractJsonSerializer(typeof(Result));
                    var res = (Result)ser.ReadObject(jsonStream);

                    return new HistoryImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = res.images[0].Copyright,
                        Description = res.images[0].Copyright,
                        ImageUrl = "http://www.bing.com" + res.images[0].URL,
                        Date = DateTime.Now.ToString("yyyy-MM-dd"),
                        AddDateTime = DateTime.Now.ToString(),
                        Locate = res.images[0].Copyright.Substring(res.images[0].Copyright.IndexOf("，") + 1, res.images[0].Copyright.IndexOf("(") - res.images[0].Copyright.IndexOf("，")-1)
                    };
                }
            }
   
        }

        public async Task<Coverstory> GetCoverstory()
        {
            using (var client = new HttpClient())
            {

                using (var jsonStream = await client.GetStreamAsync("https://cn.bing.com/cnhp/coverstory/"))
                {
                    var ser = new DataContractJsonSerializer(typeof(Coverstory));
                    return (Coverstory)ser.ReadObject(jsonStream);      
                }
            }
        }

        [DataContract]
        public class Coverstory
        {
            [DataMember(Name = "date")]
            public string date { get; set; }
            [DataMember(Name = "title")]
            public string title { get; set; }
            [DataMember(Name = "attribute")]
            public string attribute { get; set; }
            [DataMember(Name = "para1")]
            public string para1 { get; set; }
            [DataMember(Name = "para2")]
            public string para2 { get; set; }

            [DataMember(Name = "provider")]
            public string provider { get; set; }
            [DataMember(Name = "imageUrl")]
            public string imageUrl { get; set; }
            [DataMember(Name = "primaryImageUrl")]
            public string primaryImageUrl { get; set; }
            [DataMember(Name = "Country")]
            public string Country { get; set; }
            [DataMember(Name = "City")]
            public string City { get; set; }


            [DataMember(Name = "Longitude")]
            public string Longitude { get; set; }


            [DataMember(Name = "Latitude")]
            public string Latitude { get; set; }


            [DataMember(Name = "Continent")]
            public string Continent { get; set; }

            [DataMember(Name = "CityInEnglish")]
            public string CityInEnglish { get; set; }

            [DataMember(Name = "CountryCode")]
            public string CountryCode { get; set; }

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
