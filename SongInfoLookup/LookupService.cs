using System;
using System.Net;
using System.Net.Http;
using System.Xml;
using DataModel;

namespace Juke.External.Lookup
{
    public class LookupService
    {
        private const string key = "002b10709790581ecb6897234c53c875";

        public CoverArtookupResult LookupCover(Song song)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://ws.audioscrobbler.com/2.0/");
            var result = client.GetAsync("?method=album.getinfo&api_key=" + key + "&artist=" + song.Artist + "&album=" +
                                         song.Album).Result;
            var doc = new XmlDocument();
            doc.Load(result.Content.ReadAsStreamAsync().Result);
            //doc.WriteContentTo(new XmlTextWriter(Console.Out));
            var lfm = doc.GetElementsByTagName("lfm").Item(0);
            var album = lfm.FirstChild;

            var lookupResult = new CoverArtookupResult();
            var elements = doc.GetElementsByTagName("image");
            foreach (var element in album.ChildNodes)
            {
                var node = element as XmlNode;

                if (node.Name != "image")
                {
                    continue;
                }

                var size = node.Attributes[0].Value;

                switch (size)
                {
                    case "small":
                        lookupResult.SmallUrl = DownloadFile(node.InnerText);
                        break;
                    case "medium":
                        lookupResult.MediumUrl = DownloadFile(node.InnerText);
                        break;
                    case "large":
                        lookupResult.LargeUrl = DownloadFile(node.InnerText);
                        break;
                }
            }

            return lookupResult;
        }

        private byte[] DownloadFile(string url)
        {
            var client = new WebClient();
            return client.DownloadData(url);
        }
    }
}