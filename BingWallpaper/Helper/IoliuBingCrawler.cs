using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.Helper
{
    public class IoliuBingCrawler
    {
        const int MAX_LOAD_PAGE = 40;

        public static List<HistoryImage> LoadHistoryImages()
        {
            var result = new List<HistoryImage>();

            for (var i = 1; i <= MAX_LOAD_PAGE; i++)
            {
                var html = HttpHelper.SendGet("https://bing.ioliu.cn/?p=" + i);
                extractImages(result, html);
            }

            return result;
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
                        var url = "https://bing.ioliu.cn" + node.SelectSingleNode("//a[@class='mark']").Attributes["href"].Value;
                        var detailHtml = HttpHelper.SendGet(url);
                        var detailDoc = new HtmlDocument();
                        detailDoc.LoadHtml(detailHtml);
                        result.Add(new HistoryImage
                        {
                            Id = Guid.NewGuid().ToString(),
                            ImageUrl = detailDoc.DocumentNode.SelectSingleNode("//a[@class='ctrl download']").Attributes["href"].Value,
                            Title = detailDoc.DocumentNode.SelectSingleNode("//p[@class='title']").InnerText.Trim(),
                            Description = detailDoc.DocumentNode.SelectSingleNode("//p[@class='sub']").InnerText.Trim(),
                            Date = date,
                            AddDateTime = DateTime.Now.ToLongDateString(),
                            updateTime = DateTime.Now.ToLongDateString(),
                            Url = url,
                            Locate = detailDoc.DocumentNode.SelectSingleNode("//p[@class='location']").InnerText.Trim(),
                        });
                    }
                    catch
                    {

                    }
                }
            });
        }
    }
}
