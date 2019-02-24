using HtmlAgilityPack;
using System.Collections.Generic;

namespace Crawler
{
    public class HtmlParser
    {
        private readonly string _baseUrl;
        private readonly HtmlWeb _web;
        private HtmlDocument _doc;

        private static readonly string[] _blackList =
        {
            ".jpg",
            ".png",
            ".gif",
            ".mp4",
            ".pdf"
        };

        public HtmlParser(string baseUrl)
        {
            _baseUrl = baseUrl;
            _web = new HtmlWeb();
        }

        private bool IsBlacklisted(string url)
        {
            foreach(var black in _blackList)
            {
                if (url.EndsWith(black))
                    return true;
            }

            return false;
        }

        public void Load(string url = null)
        {
            _doc = _web.Load(url ?? _baseUrl);
        }

        public IEnumerable<string> GetAnchors()
        {
            var anchors = _doc.DocumentNode.SelectNodes("//a");
            if(anchors is null)
                yield break;

            foreach (var anchor in anchors)
            {
                string href = anchor.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    if (href.StartsWith(_baseUrl) && !IsBlacklisted(href))
                        yield return href;
                    else
                    {
                        if (href.StartsWith("/"))
                            yield return _baseUrl + href;
                    }
                }
            }
        }

        public string GetContents()
        {
            return _doc.DocumentNode.InnerText;
        }
    }
}
