﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InvertedIndexer
{

    class Program
    {
        public static Dictionary<string, List<int>> invertedIndex;

        static void Main(string[] args)
        {
            invertedIndex = new Dictionary<string, List<int>>();
            string folder = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "Stemmer", "stemmed");

            foreach (string file in Directory.EnumerateFiles(folder, "*.txt"))
            {
                if (file.Contains("index"))
                    continue;
                List<string> content = System.IO.File.ReadAllText(file).Split(Environment.NewLine).Distinct().ToList();
                string index = file.Substring(file.LastIndexOf("\\") + 1);
                index = index.Substring(0, index.IndexOf("."));
                addToIndex(content, index);
            }
            var ii = invertedIndex;

            string dir = Path.Combine(Environment.CurrentDirectory, "Dictionary");
            Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            foreach(var key in invertedIndex.Keys)
            {
                string file = Path.Combine(dir, key + ".txt");
                bool ignore = false;
                try
                {
                    File.Create(file).Close();
                }
                catch
                {
                    ignore = true;
                }
                if (!ignore)
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i = 1; i < 101; i++)
                    {
                        foreach(var value in invertedIndex[key])
                        {
                            if (i == value)
                            {
                                sb.Append("1");
                            }
                            else
                                sb.Append("0");
                        }
                    }

                    File.WriteAllText(file, sb.ToString());
                }

            }

            Console.ReadLine();
        }

        private static void addToIndex(List<string> words, string document)
        {
            foreach (var word in words)
            {
                if (!invertedIndex.ContainsKey(word))
                {
                    invertedIndex.Add(word, new List<int> { Convert.ToInt32(document) });
                }
                else
                {
                    invertedIndex[word].Add(Convert.ToInt32(document));
                }
            }
        }

    }
}
