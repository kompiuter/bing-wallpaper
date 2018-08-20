using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace BingWallpaper
{
    public class HistoryImageProvider
    {
        static string DataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.json");
        static string BakDataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.json.bak");
      
        static HistoryImageProvider()
        {
            historyImages = LoadHistoryImages();
            historyImages.ForEach(image =>
            {
                date2Image.Add(image.Date, image);
            });
        }

        static List<HistoryImage> historyImages;
        static Dictionary<string, HistoryImage> date2Image = new Dictionary<string, HistoryImage>();

        static List<HistoryImage> LoadHistoryImages()
        {
            try {
                string[] lines = File.ReadAllLines(DataFile);
                List<HistoryImage> images = new List<HistoryImage>();
                var ser = new DataContractJsonSerializer(typeof(HistoryImage));
                foreach(var line in lines)
                {
                    if (line.Trim().Length > 0)
                    {
                        images.Add((HistoryImage)ser.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(line))));
                    }
                }
                return images;
                }
            catch
            {
                if (File.Exists(BakDataFile))
                {
                    File.Copy(BakDataFile, DataFile);
                    return LoadHistoryImages();
                }

                return new List<HistoryImage>();
            }
        }

        /// <summary>
        ///  随机图片
        /// </summary>
        /// <returns></returns>
        public static HistoryImage getRandom()
        {
            return historyImages[new Random().Next(0, historyImages.Count - 1)];
        }

        /// <summary>
        /// 根据日期获取
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static HistoryImage GetImageByDate(string date)
        {
            if (date2Image.ContainsKey(date))
            {
                return date2Image[date];
            }
            return null;
        }

        public static HistoryImage Next(string date)
        {
            var key = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            if (date2Image.ContainsKey(key))
            {
                return date2Image[key];
            }
            return null;
        }

        public static HistoryImage Previous(string date)
        {
            var key = DateTime.Parse(date).AddDays(-1).ToString("yyyy-MM-dd");
            if (date2Image.ContainsKey(key))
            {
                return date2Image[key];
            }
            return null;
        }

        public static void AddImage(HistoryImage image)
        {         
            if (date2Image.ContainsKey(image.Date))
            {
                historyImages.Add(image);
                date2Image.Add(image.Date, image);
                Save();
            }     
        }

        public static void Save()
        {
            if (File.Exists(DataFile))
            {
                File.Delete(BakDataFile);
                File.Move(DataFile, BakDataFile);
            }
            try
            {
                using (var stream = new FileStream(DataFile, FileMode.Create))
                {
                    var ser = new DataContractJsonSerializer(typeof(HistoryImage));
                    var line = System.Text.Encoding.UTF8.GetBytes("\r\n");
                    historyImages.ForEach(image =>
                    {
                        ser.WriteObject(stream, image);
                        stream.Write(line, 0, line.Length);
                    });
                }
            }
            catch
            {
                if (File.Exists(BakDataFile))
                {
                    File.Copy(BakDataFile, DataFile);
                }
            }
        }

    }

    [DataContract]
    public class HistoryImage
    {
        [DataMember(Name = "_id")]
        public string Id { get; set; }

        [DataMember(Name = "locate")]
        public string Locate { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "addDateTime")]
        public string AddDateTime { get; set; }

        [DataMember(Name = "updateTime")]
        public string updateTime { get; set; }

        Image image;

        string realImageUrl
        {
            get
            {
                if(this.ImageUrl.StartsWith("http"))
                {
                    return this.ImageUrl;
                }

                //"http://h1.ioliu.cn/bing/Shanghai_ZH-CN10665657954_1920x1080.jpg"/
                // http://h1.ioliu.cn/bing/LakePowellStorm_ZH-CN6822865622_1920x1080.jpg

                return "http://h1.ioliu.cn/bing/" + ImageUrl.Replace("/photo/", "").Replace("?force=download","")+ "_1920x1080.jpg";
            }
        }

        static string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
        
        static HistoryImage()
        {
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
        }  

        static async Task<Image> getImageFromUrl(string url)
        {
            var cacheKey = Path.Combine(cacheDir, url.GetHashCode() + ".jpg"); 

            try
            {
                if (File.Exists(cacheKey))
                {
                    return Image.FromFile(cacheKey);
                }
            }
            catch
            {
                File.Delete(cacheKey);
            }

            using (var client = new HttpClient())
            {
                using (var imgStream = await client.GetStreamAsync(new Uri(url)))
                {
                    Image image =  Image.FromStream(imgStream);
                    image.Save(cacheKey);
                    return image;
                }
            }

        }

        /// <summary>
        /// 获取Image对象
        /// </summary>
        /// <returns></returns>
        public async Task<Image> getImage()
        {
            if (image == null)
            {
               image = await getImageFromUrl(realImageUrl);
            }
            return image;
        }
    }



}
