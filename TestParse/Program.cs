using System;
using System.Diagnostics;  
using System.Net;          
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace TestParse
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> categoryNames = new List<string> {  "monitoring", "insulin", "meters", "test" };
           


            foreach (string categoryName in categoryNames)
            {
                int page = 1;
                List<string> allCategoryLinks = new List<string>();
                List<string> currentPageLinks = getLinksFromCategory(categoryName + makePage(page));
                
                while (currentPageLinks.Count>0)
                {
                    allCategoryLinks.AddRange(currentPageLinks);
                    page++;
                   currentPageLinks = getLinksFromCategory(categoryName + makePage(page));
                }
                Console.WriteLine(categoryName);
                Console.WriteLine(String.Join("\n", allCategoryLinks));

            }


            Console.ReadLine();
        }

        public static List<string> getLinksFromCategory(string categoryName)
        {
            try
            {
                WebClient wc = new WebClient();
                string htmlStr = wc.DownloadString("https://diaexpert.ua/product-category/" + categoryName + "/");

                var txtTitle = FindText(htmlStr, @"<ul class=" + '\u0022' + @"products columns-4" + '\u0022' + @">", @"</ul>");


                //Регулярное выражение для поиска A тега
                var regexpATag = new Regex(@"<a[^<>]*>[^<]*<\/a>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //Регулярное выражение для поиска href свойства
                var regexpHref = new Regex(@"href\s*=\s*[""'](.*?)[""']", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                var matches = regexpHref.Matches(txtTitle);

                var links = new List<string>();
                foreach (Match match in matches)
                {
                    links.Add(FindText(match.Value, @"href=" + '\u0022', @"" + '\u0022'));
                };
                //LINQ запрос на сортировку и уникализацию
                links = links.Distinct().OrderBy(el => el).ToList().FindAll(el => el.Contains("http"));
                return links;
            }
            catch
            {
                return new List<string>();
            }
            
        }
        public static string makePage(int number)
        {
            if(number == 1)
            {
                return "";
            }
            return "/page/" + number + "/";
        }
        public static String FindText(string source, string prefix, string suffix)
        {
            var prefixPosition = source.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            var suffixPosition = source.IndexOf(suffix, prefixPosition + prefix.Length, StringComparison.OrdinalIgnoreCase);

            if ((prefixPosition >= 0) && (suffixPosition >= 0) && (suffixPosition > prefixPosition) && ((prefixPosition + prefix.Length) <= suffixPosition))
            {
                return source.Substring(
                                prefixPosition + prefix.Length,
                                suffixPosition - prefixPosition - prefix.Length
                    );
            }
            else
            {
                return String.Empty;
            }
        }

    }
}
