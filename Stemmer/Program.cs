using System;
using System.IO;
using System.Text;

namespace Stemmer
{
    class Program
    {
        private static string BaseFileDirectory;
        private static string StemmedFileDirectory;
        static string GetCurrentDirectory()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        static void Initialize()
        {
            BaseFileDirectory = Path.Combine(GetCurrentDirectory(), "docs");
            Directory.CreateDirectory(BaseFileDirectory);
            StemmedFileDirectory = Path.Combine(GetCurrentDirectory(), "stemmed");
            Directory.CreateDirectory(StemmedFileDirectory);
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Initialize();
            string[] baseFiles = Directory.GetFiles(BaseFileDirectory);
            foreach(string file in baseFiles)
            {
                string contents = File.ReadAllText(file);
                StringBuilder builder = new StringBuilder();
                
                foreach (string word in contents.Split(new char[] { ' ', '.', ',', '?', '!', ':' }))
                {
                    string clearedWord = word.Replace(" ", "").Replace("\t", "").Replace("\n", "");
                    if (string.IsNullOrWhiteSpace(clearedWord))
                        continue;
                    string stemmedWord = Porter.Stemm(clearedWord);
                    if (string.IsNullOrWhiteSpace(stemmedWord))
                        continue;
                    builder.AppendLine(stemmedWord);
                }
                string stemmedFile = Path.Combine(StemmedFileDirectory, Path.GetFileName(file));
                File.WriteAllText(stemmedFile, builder.ToString());
            }

            Console.WriteLine("Completed!");
            Console.ReadLine();
        }
    }
}
