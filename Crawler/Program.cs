using System;
using System.Collections.Generic;
using System.IO;

namespace Crawler
{
    class Program
    {
        private const string BaseUrl = "https://tutorialzine.com";
        private static string BaseFile;
        private static string DocumentDirectory;

        private static Queue<string> _urls = new Queue<string>();
        private static HashSet<string> _crawledUrls;
        private static HtmlParser _parser = new HtmlParser(BaseUrl);

        static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        static void Initialize()
        {
            DocumentDirectory = GetCurrentDirectory() + "/docs";
            Directory.CreateDirectory(DocumentDirectory);
            BaseFile = DocumentDirectory + "/index.txt";

            if (!File.Exists(BaseFile))
                File.Create(BaseFile).Close();

            _crawledUrls = new HashSet<string>(File.ReadAllLines(BaseFile));
        }

        static void Main(string[] args)
        {
            Initialize();

            using (StreamWriter baseWriter = File.AppendText(BaseFile))
            {
                Crawl(BaseUrl, baseWriter);

                while (_urls.Count != 0)
                {
                    if (_crawledUrls.Count > 100)
                        break;
                    string url = _urls.Dequeue();
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
                if (!_crawledUrls.Contains(anchor) && !_urls.Contains(anchor))
                {
                    _urls.Enqueue(anchor);
                    Console.WriteLine(anchor);
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
