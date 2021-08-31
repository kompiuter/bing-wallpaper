using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BingWallpaper
{
    public class HttpHelper
    {
    private static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //用户https请求
            return true; //总是接受
        }

        public static string SendPost(string url, string data)
        {
            return Send(url, "POST", data, null);
        }

        public static string SendGet(string url)
        {
            return Send(url, "GET", null, null);
        }

        public static string SendGet(string url, HttpConfig config)
        {
            return Send(url, "GET", null, config);
        }

        public static string Send(string url, string method, string data, HttpConfig config)
        {
            if (config == null) config = new HttpConfig();
            string result;
            using (HttpWebResponse response = GetResponse(url, method, data, config))
            {
                Stream stream = response.GetResponseStream();

                if (!string.IsNullOrEmpty(response.ContentEncoding))
                {
                    if (response.ContentEncoding.Contains("gzip"))
                    {
                        stream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    else if (response.ContentEncoding.Contains("deflate"))
                    {
                        stream = new DeflateStream(stream, CompressionMode.Decompress);
                    }
                }

                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    int count;
                    byte[] buffer = new byte[4096];
                    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }
                    bytes = ms.ToArray();
                }

                #region 检测流编码
                Encoding encoding;

                //检测响应头是否返回了编码类型,若返回了编码类型则使用返回的编码
                //注：有时响应头没有编码类型，CharacterSet经常设置为ISO-8859-1
                if (!string.IsNullOrEmpty(response.CharacterSet) && response.CharacterSet.ToUpper() != "ISO-8859-1")
                {
                    encoding = Encoding.GetEncoding(response.CharacterSet == "utf8" ? "utf-8" : response.CharacterSet);
                }
                else
                {
                    //若没有在响应头找到编码，则去html找meta头的charset
                    result = Encoding.Default.GetString(bytes);
                    //在返回的html里使用正则匹配页面编码
                    Match match = Regex.Match(result, @"<meta.*charset=""?([\w-]+)""?.*>", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        encoding = Encoding.GetEncoding(match.Groups[1].Value);
                    }
                    else
                    {
                        //若html里面也找不到编码，默认使用utf-8
                        encoding = Encoding.GetEncoding(config.CharacterSet);
                    }
                }
                #endregion

                result = encoding.GetString(bytes);
            }
            return result;
        }

        private static HttpWebResponse GetResponse(string url, string method, string data, HttpConfig config)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Referer = config.Referer;
            //有些页面不设置用户代理信息则会抓取不到内容
            request.UserAgent = config.UserAgent;
            request.Timeout = config.Timeout;
            request.Accept = config.Accept;
            request.Headers.Set("Accept-Encoding", config.AcceptEncoding);
            request.ContentType = config.ContentType;
            request.KeepAlive = config.KeepAlive;

            if (config.Cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                var cookies = config.Cookie.Split(';').ToList();
                cookies.ForEach(cookie =>
                {
                    var cookieParts = cookie.Split('=');
                    if (cookieParts.Length == 2)
                    {
                        request.CookieContainer.Add(new Cookie(cookieParts[0].Trim(), cookieParts[1].Trim()){ Domain = request.RequestUri.Host });
                    }
                });

            }

            if (url.ToLower().StartsWith("https"))
            {
                //这里加入解决生产环境访问https的问题--Could not establish trust relationship for the SSL/TLS secure channel
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidate);
            }


            if (method.ToUpper() == "POST")
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);

                    if (config.GZipCompress)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                            {
                                gZipStream.Write(bytes, 0, bytes.Length);
                            }
                            bytes = stream.ToArray();
                        }
                    }

                    request.ContentLength = bytes.Length;
                    request.GetRequestStream().Write(bytes, 0, bytes.Length);
                }
                else
                {
                    request.ContentLength = 0;
                }
            }

            var response =  (HttpWebResponse)request.GetResponse();
            if (config.Cookie != null) { 
                // 更新cookie
                config.Cookie = request.CookieContainer.GetCookieHeader(request.RequestUri);
            }
            return response;

        }


        private static readonly Encoding DEFAULTENCODE = Encoding.UTF8;



        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HttpUploadFile(string url, string file, NameValueCollection data, HttpConfig config)
        {
            return HttpUploadFile(url, file, data, DEFAULTENCODE, config);
        }

        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpUploadFile(string url, string file, NameValueCollection data, Encoding encoding, HttpConfig config)
        {
            HttpFile[] files = new HttpFile[1] { new HttpFile { FileFieldName = "file0", Path = file } };

            return HttpUploadFile(url, files, data, encoding, config);
        }

        public static string HttpUploadFile(string url, HttpFile file, NameValueCollection data, Encoding encoding, HttpConfig config)
        {
            HttpFile[] files = new HttpFile[1] { file };

            return HttpUploadFile(url, files, data, encoding, config);
        }


        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpUploadFile(string url, HttpFile[] files, NameValueCollection data, Encoding encoding, HttpConfig config)
        {
            
            try
            {
                string boundary = "------WebKitFormBoundary"+ DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                //1.HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 1000 * 400;
                request.Accept = config.Accept;
                if (config.XMLHttpRequest)
                {
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                }
                request.Referer = config.Referer;
                if (config.Origin != null) {
                    request.Headers.Add("Origin", config.Origin);
                }
                request.UserAgent = config.UserAgent;

                using (Stream stream = request.GetRequestStream())
                {
                    //1.1 key/value
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    if (data != null)
                    {
                        foreach (string key in data.Keys)
                        {
                            stream.Write(boundarybytes, 0, boundarybytes.Length);
                            string formitem = string.Format(formdataTemplate, key, data[key]);
                            byte[] formitembytes = encoding.GetBytes(formitem);
                            stream.Write(formitembytes, 0, formitembytes.Length);
                        }
                    }

                    //1.2 file  image/jpeg
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    for (int i = 0; i < files.Length; i++)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string header = string.Format(headerTemplate, files[i].FileFieldName, Path.GetFileName(files[i].Path), files[i].ContentType);
                        byte[] headerbytes = encoding.GetBytes(header);
                        stream.Write(headerbytes, 0, headerbytes.Length);
                        using (FileStream fileStream = new FileStream(files[i].Path, FileMode.Open, FileAccess.Read))
                        {
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                stream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }

                    //1.3 form end
                    stream.Write(endbytes, 0, endbytes.Length);
                }
                //2.WebResponse
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    return stream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

}

    public class HttpConfig
    {
        public string Referer { get; set; }

        /// <summary>
        /// 默认(text/html)
        /// </summary>
        public string ContentType { get; set; }

        public string Accept { get; set; }

        public string AcceptEncoding { get; set; }

        /// <summary>
        /// 超时时间(毫秒)默认100000
        /// </summary>
        public int Timeout { get; set; }

        public string UserAgent { get; set; }

        /// <summary>
        /// POST请求时，数据是否进行gzip压缩
        /// </summary>
        public bool GZipCompress { get; set; }

        public bool KeepAlive { get; set; }

        public string CharacterSet { get; set; }
        public string Origin { get; internal set; }

        public bool XMLHttpRequest { get; set; } = false;

        public string Cookie { get; set; }

        public HttpConfig()
        {
            this.Timeout = 100000;
            this.ContentType = "text/html; charset=" + Encoding.UTF8.WebName;
            this.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.117 Safari/537.36";
            this.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            this.AcceptEncoding = "gzip,deflate";
            this.GZipCompress = false;
            this.KeepAlive = true;
            this.CharacterSet = "UTF-8";
        }
    }


    public class HttpFile
    {
        public string FileFieldName { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
