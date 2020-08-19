using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace YoutubeThumbnailScraper
{
    class Program
    {
        static List<string> m_urls = new List<string>();
        static string searchString = "og:image";
        static int processedImages = 0;

        static void Main(string[] args)
        {
            Console.Title = "Thumbnail Scraper";
            var _parent = Directory.GetParent("YoutubeThumbnailScraper");
            var _par = Directory.GetParent(_parent.FullName);
            foreach (var file in _par.GetFiles())
            {
                if (file.Name.Contains(".jpg"))
                {
                    processedImages++;
                }
            }
            var _string = _par.FullName + @"\URL.txt";
            if(ReadURLs(_string))
            {
                foreach (var img in m_urls)
                {
                    try
                    {
                        SaveImage(img, _par.FullName + @"\" + $"fileName{processedImages}.jpg", ImageFormat.Jpeg);
                        processedImages++;
                    }
                    catch (ExternalException ex)
                    {
                        Console.WriteLine($"Error saving image: {ex}");
                        Console.WriteLine("Check file Format");
                        Console.ReadKey();
                        return;
                    }
                    catch (ArgumentNullException nullEx)
                    {
                        Console.WriteLine($"Error saving image!");
                        Console.WriteLine($"Exception message: {nullEx}");
                        Console.WriteLine($"Problem url: {img}");
                    }
                }
            }

            Console.WriteLine("Completed without Error.");
            Console.ReadKey();
        }

        static void SaveImage(string _url, string _filename, ImageFormat format)
        {
            using (WebClient _webClient = new WebClient())
            {
                var imageURL = ReturnImageUrl(_url);
                Console.WriteLine($"Downloading thumbnail from: {imageURL}");
                _webClient.DownloadFile(imageURL + ".jpg", _filename);
            }
        }

        static string ReturnImageUrl(string url)
        {
            string outString = "";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var htmlCharArray = doc.DocumentNode.InnerHtml;
            int index = htmlCharArray.IndexOf(searchString);

            var ar = htmlCharArray.Split("<");
            int _index = 0;
            foreach (var parentList in ar)
            {
                if (parentList.Contains(searchString))
                {
                    foreach (var subList in parentList.Split(">"))
                    {
                        if (subList.Contains(searchString))
                        {
                            var _urlStartPoint = subList.IndexOf("https://");
                            if (_urlStartPoint > 0)
                            {
                                var preOutString = subList.Substring(_urlStartPoint);
                                var temp = preOutString.Split(".jpg");
                                outString = temp[0];
                            }
                        }
                        _index++;
                    }
                }
            }
            return outString;
        }
        static bool ReadURLs(string _urls)
        {
            string errorString = "Insert URL here";
            do
            {
                try
                {
                    StreamReader sr = new StreamReader(_urls);
                    var _length = Convert.ToInt32(sr.ReadLine());
                    for (int i = 0; i < _length; i++)
                    {
                        m_urls.Add(sr.ReadLine());
                    }
                    sr.Close();
                    if (m_urls.Count == 0)
                    {
                        Console.WriteLine("No urls found, check file.");
                        Console.ReadKey();
                        return false;
                    }
                }
                catch (Exception)
                {
                    StreamWriter sw = new StreamWriter(_urls, true);
                    sw.WriteLine("1");
                    sw.WriteLine(errorString);
                    sw.Close();
                    Console.WriteLine("Text file failed to load. Created new file.");
                }
            } while (m_urls.Count == 0);
            if (m_urls[0] == errorString)
            {
                return false;
            }
            return true;
        }
    }
}
