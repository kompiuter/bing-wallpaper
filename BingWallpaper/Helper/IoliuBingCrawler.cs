using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BingWallpaper.Helper
{
    public class IoliuBingCrawler
    {
        const int MAX_LOAD_PAGE = 40;

        public static List<HistoryImage> LoadHistoryImages()
        {
            var result = new List<HistoryImage>();
            try
            {
                for (var i = 1; i <= MAX_LOAD_PAGE; i++)
                {
                    var html = HttpHelper.SendGet("https://bing.ioliu.cn/?p=" + i);
                    extractImages(result, html);
                }
            }
            catch
            {

            }
            return result;
        }

        public static List<HistoryImage> LoadLatestDaysImages()
        {
            var result = new List<HistoryImage>();
            try
            {
                var html = HttpHelper.SendGet("https://bing.ioliu.cn/");
                extractImages(result, html);
            }
            catch
            {

            }
            return result;
        }

        static string SelectTextNode(HtmlDocument htmlDocument,string xpath)
        {
            var node = htmlDocument.DocumentNode.SelectSingleNode(xpath);
            if(node !=null)
            {
                return node.InnerText.Trim();
            }
            return string.Empty;
        }

        private static void extractImages(List<HistoryImage> result, string indexPageHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(indexPageHtml);
            var items = doc.DocumentNode.SelectNodes("//div[@class='item']");
            //HistoryImageProvider
            items.ToList().ForEach(node =>
            {
                var date = node.SelectSingleNode(".//em[1]").InnerText;
                if (!HistoryImageProvider.IsExist(date))
                {
                    try
                    {
                        var url = "https://bing.ioliu.cn" + node.SelectSingleNode(".//a[@class='mark']").Attributes["href"].Value;
                        result.Add(fetchSpecDayWallpaper(date, url));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }

        private static HistoryImage fetchSpecDayWallpaper(string date, string url)
        {
            var detailHtml = HttpHelper.SendGet(url);
            var detailDoc = new HtmlDocument();
            detailDoc.LoadHtml(detailHtml);
            var result =  new HistoryImage
            {
                Id = Guid.NewGuid().ToString(),
                ImageUrl = detailDoc.DocumentNode.SelectSingleNode("//a[@class='ctrl download']").Attributes["href"].Value,
                Title = SelectTextNode(detailDoc, "//p[@class='title']"),
                Description = SelectTextNode(detailDoc, "//p[@class='sub']"),
                Date = date,
                AddDateTime = DateTime.Now.ToLongDateString(),
                updateTime = DateTime.Now.ToLongDateString(),
                Url = url,
                Locate = SelectTextNode(detailDoc, "//p[@class='location']")
            };

            if(result.Locate.Length == 0)
            {
                result.Locate = result.Title.GetBetween("，", "(");
            }

            return result;
        }
    }
}
