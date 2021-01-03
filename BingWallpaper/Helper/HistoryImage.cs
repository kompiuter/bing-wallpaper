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
        }

        static SortedList<string, HistoryImage> historyImages;

        static SortedList<string, HistoryImage> LoadHistoryImages()
        {
            try
            {
                string[] lines = File.ReadAllLines(DataFile);
                // List<HistoryImage> images = new List<HistoryImage>();
                SortedList<string, HistoryImage> images = new SortedList<string, HistoryImage>();
                var ser = new DataContractJsonSerializer(typeof(HistoryImage));
                foreach (var line in lines)
                {
                    if (line.Trim().Length > 0)
                    {
                        var image = (HistoryImage)ser.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(line)));
                        images.Add(image.Date, image);
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

                return new SortedList<string, HistoryImage>();
            }
        }

        /// <summary>
        ///  随机图片
        /// </summary>
        /// <returns></returns>
        public static HistoryImage getRandom()
        {
            return historyImages.Values[new Random().Next(0, historyImages.Count - 1)];
        }

        /// <summary>
        /// 根据日期获取
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static HistoryImage GetImageByDate(string date)
        {
            if (historyImages.ContainsKey(date))
            {
                return historyImages[date];
            }
            return null;
        }

        public static bool IsExist(string date)
        {
            return historyImages.ContainsKey(date);
        }

        public static HistoryImage Next(string date)
        {
            var index = historyImages.IndexOfKey(date) + 1;
            if(index <= historyImages.Count - 1)
            {
                return historyImages.Values[index];
            }
            return null;
        }

        public static HistoryImage Previous(string date)
        {
            var index = historyImages.IndexOfKey(date) - 1;
            if (index >= 0)
            {
                return historyImages.Values[index];
            }
            return null;
        }

        public static void AddImage(HistoryImage image)
        {
            if (!historyImages.ContainsKey(image.Date))
            {
                historyImages.Add(image.Date, image);
                Save();
            }
        }

        public static void AddBatch(List<HistoryImage> images)
        {
            foreach (var image in images)
            {
                if (!historyImages.ContainsKey(image.Date))
                {
                    historyImages.Add(image.Date, image);
                }
            }
            Save();

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
                    for (var i = historyImages.Values.GetEnumerator(); i.MoveNext();)
                    {
                        var image = i.Current;
                        ser.WriteObject(stream, image);
                        stream.Write(line, 0, line.Length);
                    }
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
    public class HistoryImage : IComparable
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
                if (this.ImageUrl.StartsWith("http"))
                {
                    return this.ImageUrl;
                }

                //"http://h1.ioliu.cn/bing/Shanghai_ZH-CN10665657954_1920x1080.jpg"/
                // http://h1.ioliu.cn/bing/LakePowellStorm_ZH-CN6822865622_1920x1080.jpg

                return "http://h1.ioliu.cn/bing/" + ImageUrl.Replace("/photo/", "").Replace("?force=download", "") + "_1920x1080.jpg";
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
                    Image image = Image.FromStream(imgStream);
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

        public void SaveToFile(string file)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    var ser = new DataContractJsonSerializer(typeof(HistoryImage));
                    ser.WriteObject(stream, image);

                }
            }
            catch
            {

            }
        }

        public static HistoryImage LoadFromFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {

                    var ser = new DataContractJsonSerializer(typeof(HistoryImage));
                    var line = File.ReadAllText(file);
                    var obj = (HistoryImage)ser.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(line)));
                    return obj;
                }
            }
            catch
            {

            }
            return null;

        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            return (obj as HistoryImage).Date.CompareTo(this.Date);
        }
    }

}
