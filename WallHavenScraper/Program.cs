using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WallHavenScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                string urlAddress =
                    "https://alpha.wallhaven.cc/search?categories=111&purity=100&resolutions=1920x1080&sorting=random&order=desc&page=2";

                var data = _loadHtmlContent(urlAddress);
     
                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                //For some weird reason you have to save it for it to work properly
                doc.Save("test.html");

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[contains(@class,'preview')]"))
                {
                    var att = link.Attributes["href"];

                    var url = att.Value;

                    var picHtml = _loadHtmlContent(url);
                    
                    doc.LoadHtml(picHtml);
                    //For some weird reason you have to save it for it to work properly
                    doc.Save("pic.html");

                    var picture = doc.DocumentNode.SelectSingleNode("//img[contains(@id,'wallpaper')]");

                    var pictureLink = picture.Attributes["src"].Value;
                    var pictureName = pictureLink.Replace("//wallpapers.wallhaven.cc/wallpapers/full/wallhaven-", "");

                    pictureLink = "https:" + pictureLink;

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(pictureLink, @"c:\WallHaven\" + pictureName);
                    }

                    Console.WriteLine(pictureLink);
                }
            }
        }

        private static string _loadHtmlContent(string urlAddress)
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            return data;
        }
    }
}
