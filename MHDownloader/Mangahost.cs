using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MHostDownlaoder
{



    public class Mangahost
    {
        static string _urlSearch = "https://mangahost2.com/find/";
        static string[] _ignore = { "cred", "créd", "avi", "rec", "crd", "hist" };
        static string[] _ignoreExtra = { "omake", "extra", "especial", "special" };

        public HtmlNodeCollection SearchManga(String mangaName)
        {
            var doc = new HtmlWeb().Load($"{_urlSearch}{mangaName.Replace(' ', '+')}");

            var mangas = doc.DocumentNode.SelectNodes("//h4[contains(@class, 'entry-title')]/a");

            return mangas;
        }

        public IEnumerable<HtmlNode> MangaPage(string mangaLink)
        {

            var doc = new HtmlWeb().Load(mangaLink);
            var chapters = doc.DocumentNode.SelectNodes("//a[contains(@class, 'cap')]");
            var smallChapters = chapters.Select(x => x);

            return smallChapters.Reverse();
        }
        private IEnumerable<HtmlNode> GetListPages(string chapterLink)
        {

            var doc = new HtmlWeb().Load(chapterLink);

            var imagePages = doc.DocumentNode.SelectNodes("//div[contains(@id, slider)]/a/img");

            if (imagePages == null)
            {
                return new List<HtmlNode>();
            }

            return imagePages.OrderBy(y => y.GetAttributeValue("id", string.Empty));

        }
        private void DownloadChapter(HtmlNode chapter, string mangaName, string chapterName, string mangaLink)
        {
            var idChapter = chapter.GetAttributeValue("id", string.Empty);

            if (idChapter.Contains("_") && !_ignoreExtra.Any(s => idChapter.ToLower().Contains(s)))
            {
                idChapter = idChapter.Replace('_', '.');
            }

            var pages = GetListPages($"{mangaLink}/{idChapter}");
            chapterName = chapterName.Replace("#", string.Empty);
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var page in pages)
            {
                var url = page.GetAttributeValue("src", string.Empty);
                var chaps = url.Split('/');
                var imageName = chaps[chaps.Length - 1];

                if (!_ignore.Any(s => imageName.ToLower().Contains(s)))
                {
                    var client = Util.GetWebClient();

                    Console.WriteLine($"Baixando o {chapterName} página {imageName}.....");
                    Util.DownlaodImage(client, mangaName, chapterName, imageName, url);
                }
            }
            Util.CloseWebClient();
        }
        public void DownloadManga(int indice, List<HtmlNode> chapters, List<string> chaptersName, string mangaName, string mangaLink)
        {

            if (indice > -1)
            {
                DownloadChapter(chapters[indice], mangaName, chaptersName[indice], mangaLink);
                return;
            }

            var i = 0;
            foreach (var chapter in chapters)
            {
                DownloadChapter(chapter, mangaName, chaptersName[i], mangaLink);
                i++;
            }

        }


    }
}
