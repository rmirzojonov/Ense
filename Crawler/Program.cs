using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Crawler
{
    class Program
    {
        private const string BaseUrl = "https://ru.wikipedia.org";
        private const string StartingUrl = "https://ru.wikipedia.org/wiki/Индекс_Хирша";
        private static string BaseFile;
        private static string DocumentDirectory;

        private static Queue<string> _urls = new Queue<string>();
        private static HashSet<string> _crawledUrls;
        private static HtmlParser _parser = new HtmlParser(BaseUrl);

        static string GetCurrentDirectory()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        static void Initialize()
        {
            DocumentDirectory = Path.Combine(GetCurrentDirectory(), "docs");
            Directory.CreateDirectory(DocumentDirectory);
            BaseFile = Path.Combine(DocumentDirectory, "index.txt");

            if (!File.Exists(BaseFile))
                File.Create(BaseFile).Close();

            _crawledUrls = new HashSet<string>(File.ReadAllLines(BaseFile));
        }

        static void Main(string[] args)
        {
            Initialize();

            using (StreamWriter baseWriter = File.AppendText(BaseFile))
            {
                Crawl(StartingUrl, baseWriter);

                while (_urls.Count != 0)
                {
                    if (_crawledUrls.Count > 99)
                        break;
                    string url = _urls.Dequeue();
                    url = HttpUtility.UrlDecode(url);
                    Crawl(url, baseWriter);
                }
            }

            Console.WriteLine("\nCompleted!");
            Console.ReadKey();
        }

        static void Crawl(string url, StreamWriter baseWriter)
        {
            baseWriter.WriteLine(url);
            _crawledUrls.Add(url);
            _parser.Load(url);

            foreach (var anchor in _parser.GetAnchors())
            {
                string decoded = HttpUtility.UrlDecode(anchor);
                if (!_crawledUrls.Contains(decoded) && !_urls.Contains(decoded))
                {
                    _urls.Enqueue(decoded);
                    Console.WriteLine(decoded);
                }
            }

            string filename = $"{DocumentDirectory}/{_crawledUrls.Count}.txt";
            using (StreamWriter writer = File.AppendText(filename))
            {
                writer.Write(_parser.GetContents());
            }
        }
    }
}
