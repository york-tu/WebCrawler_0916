using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace WebCrawler_0916
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentdate = DateTime.Now.ToString("yyyy-MM-dd_HHmmss_ffff");
            string Upperfolderpath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")); // 存檔路徑 (程式資料夾第一層)

            
            for (int level = 0; level < 5; level++) // 撈 5 層
            {
                List<string> urlLevelList = new List<string>();

                StreamReader pathLinks = new StreamReader($@"{Upperfolderpath}\URL_level_{level}.txt"); // 讀指定txt
                StreamWriter LinkReport = new StreamWriter($@"{Upperfolderpath}\URL_level_{level + 1}.txt", true); // 寫入指定路徑text (true: 接續寫資料)
                
                string str_read = pathLinks.ReadToEnd(); // 一次讀檔
                pathLinks.Close(); // 關txt

                List<string> URLList = new List<string>();
                if (level >= 1)
                {
                    StreamReader previousLevelLinks = new StreamReader($@"{Upperfolderpath}\URL_level_{level - 1}.txt"); // 讀前一層txt檔
                    string previous_level_links = previousLevelLinks.ReadToEnd();
                    previousLevelLinks.Close();
                    List<string> previousLevelLinksArray = (previous_level_links.Split("\r\n")).ToList();
                    var currentList = (str_read.Split("\r\n")).ToList();
                    URLList = currentList.Except(previousLevelLinksArray).ToList();
                }
                else
                {
                    URLList = str_read.Split("\r\n").ToList();
                }

                int count = 1;
                foreach (var weblink in URLList)
                {
                    string pattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?"; // 判斷網址URL
                    try
                    {
                        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(weblink);
                        request1.Method = "GET";
                        request1.AllowAutoRedirect = true;
                        HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

                        StreamReader reader = new StreamReader(response1.GetResponseStream());
                        string Code = reader.ReadToEnd(); //爬出網頁原始碼並存入string "code"
                        MatchCollection matchSources = Regex.Matches(Code, pattern); // 找出所有URL字串
                        
                        for (int i = 0; i < matchSources.Count; i++)
                        {
                            urlLevelList.Add( matchSources[i].ToString());
                        }

                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"第 {count} 筆_{ e.Message}");
                    }
                    count++;
                }

                var briefList = urlLevelList.Distinct().ToArray();

                int listCount = 1;
                foreach (var link in briefList)
                {
                    if (link.Contains("esunbank"))
                    {
                        LinkReport.WriteLine(link);
                        listCount++;
                    }  
                }
                LinkReport.Close();
            }
        }
    }
}
