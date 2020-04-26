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
        static string _urlSearch = "https://mangahost.site/find/";
        static string[] _ignore = { "cred", "créd", "avi", "rec", "crd","hist" };

        public HtmlNodeCollection SearchManga(String mangaName)
        {
            var doc = new HtmlWeb().Load($"{_urlSearch}{mangaName.Replace(' ', '+')}");

            var mangas = doc.DocumentNode.SelectNodes("//h3[contains(@class, 'entry-title')]/a");

            return mangas;
        }

        public IEnumerable<HtmlNode> MangaPage(string mangaLink)
        {

            var doc = new HtmlWeb().Load(mangaLink);
            var chapters = doc.DocumentNode.SelectNodes("//a[contains(@class, 'capitulo')]");

            if (chapters != null)
            {
                var smallChapters = chapters.Select(x => x);

                return smallChapters.Reverse();
            }

            chapters = doc.DocumentNode.SelectNodes("//a[contains(@data-html, 'true')]");

            var bigChapters = new List<HtmlNode>();
            foreach (var cc in chapters)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(cc.GetAttributeValue("data-content", string.Empty));
                htmlDoc.DocumentNode.SelectNodes("//div/a").First();

                bigChapters.Add(htmlDoc.DocumentNode.SelectNodes("//div/a").First());
            }

            bigChapters.Reverse();
            return bigChapters;
        }
        private IEnumerable<HtmlNode> GetListPages(string chapterLink)
        {

            var doc = new HtmlWeb().Load(chapterLink);
            var scripts = doc.DocumentNode.SelectNodes("//div[contains(@class, content-site)]/script");

            var scriptIndex = scripts.Where(x => x.InnerText.Contains("var images = ")).FirstOrDefault();

            if (scriptIndex == null)
            {
                throw new Exception("Não foi encontrado nenhuma página..");
            }

            var script = scriptIndex.InnerText.Substring(scriptIndex.InnerText.IndexOf("var images = "));
            script = script.Substring(13, script.IndexOf("];"));

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(script);

            return htmlDoc.DocumentNode.SelectNodes("//img").OrderBy(y => y.GetAttributeValue("id", string.Empty));

        }
        private void DownloadChapter(HtmlNode chapter, string mangaName, string chapterName)
        {
            var pages = GetListPages(chapter.GetAttributeValue("href", string.Empty));
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
        public void DownloadManga(int indice, List<HtmlNode> chapters, List<string> chaptersName, string mangaName)
        {

            if (indice > -1)
            {
                DownloadChapter(chapters[indice], mangaName, chaptersName[indice]);
                return;
            }

            var i = 0;
            foreach (var chapter in chapters)
            {
                DownloadChapter(chapter, mangaName, chaptersName[i]);
                i++;
            }

        }


    }
}
