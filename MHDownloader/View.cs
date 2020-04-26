using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MHostDownlaoder
{
    public class View
    {
        private Mangahost _mangaHost;



        public View()
        {
            _mangaHost = new Mangahost();
        }
        public void MenuPrincipal()
        {
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("               | MangaHost Downloader |               ");
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("1 - Buscar Mangá");
            //Console.WriteLine("2 - Configurações");
            //Console.WriteLine("0 - Sair");
            //Console.Write("> ");
            //var opcao = Console.ReadLine();
            //OptionsCase(opcao);
            BuscaMangas();

        }
        private void BuscaMangas()
        {
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("               | MangaHost Downloader |               ");
            Console.WriteLine("------------------------------------------------------");
            Console.Write("Digite o nome do Mangá: ");
            var mangaName = Console.ReadLine();

            var mangas = _mangaHost.SearchManga(mangaName);
            var i = 1;

            Console.WriteLine("");

            if (mangas == null || mangas.Count < 1)
            {
                Console.WriteLine("Não foi encontrado nenhum mangá.....");
                Console.ReadKey();
                Console.Clear();
                BuscaMangas();
                return;
            }

            foreach (var manga in mangas)
            {
                Console.WriteLine($"{i} - {manga.InnerText}");
                i++;
            }
            Console.Write("Digite o índice do Mangá que deseja baixar, ou digite [S] para Encerrar: ");
            var opcao = Console.ReadLine();

            var opcaoInt = 0;
            var isnumber = int.TryParse(opcao, out opcaoInt);

            if (opcao.ToLower().Contains("s"))
            {
                Encerrar();
                return;
            }

            if (!isnumber || opcaoInt > mangas.Count || opcaoInt <= 0)
            {
                OpcaoInvalida();
                BuscaMangas();
                return;
            }
            SelectedManga(mangas[opcaoInt - 1]);
        }
        private void SelectedManga(HtmlNode manga)
        {
            Console.WriteLine($"------------------------------------------------------");
            Console.WriteLine($"    |{manga.InnerText}|");
            Console.WriteLine($"------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"0 - Todos capítulos de {manga.InnerText}");

            var chapters = _mangaHost.MangaPage(manga.GetAttributeValue("href", string.Empty));


            var i = 1;
            var chapterCount = chapters.Count();
            Console.ForegroundColor = ConsoleColor.Blue;
            var chaptersName = new List<string>();

            foreach (var chapter in chapters)
            {
                var chapterName = chapter.GetAttributeValue("title", string.Empty);
                Util.ReplaceList.ToList().ForEach(x => chapterName = chapterName.Replace(x, string.Empty).Trim());

                var chapterDouble = 0d;
                var isDouble = double.TryParse(chapterName.Substring(1, 2), out chapterDouble);

                if (i < 15 && isDouble && chapterDouble > 0 && chapterDouble < 10)
                    chapterName = chapterName.Substring(0, 3).Any(char.IsDigit) ? chapterName.Replace("#", "#0") : chapterName;

                chapterDouble = 0d;
                isDouble = double.TryParse(chapterName.Substring(1, 3), out chapterDouble);

                if (chapterCount >= 100 && isDouble && chapterDouble > 0 && chapterDouble < 100)
                    chapterName = chapterName.Replace("#", "#0");


                if (i >= 100)
                {
                    Console.WriteLine($"{i} - {chapterName.Trim()}");
                }
                else
                {
                    Console.WriteLine(i >= 10 ? $"0{i} - {chapterName.Trim()}" : $"00{i} - {chapterName.Trim()}");
                }
                chaptersName.Add(chapterName);
                i++;
            }
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Digite o índice do Capítulo para baixar, ou digite [S] para Encerrar: ");
            var opcao = Console.ReadLine();

            if (opcao.ToLower().Contains("s"))
            {
                Encerrar();
                return;
            }

            var opcaoInt = 0;
            var isnumber = int.TryParse(opcao, out opcaoInt);

            if (!isnumber || opcaoInt > chapters.Count() || opcaoInt < 0)
            {
                OpcaoInvalida();
                SelectedManga(manga);
                return;
            }

            _mangaHost.DownloadManga(opcaoInt - 1, chapters.ToList(), chaptersName,manga.InnerText);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Download Concluído....");
            Console.ReadLine();
            MenuPrincipal();

        }

        private void OpcaoInvalida()
        {
            Console.Write("Opção inválida..");
            Console.ReadKey();
            Console.Clear();
        }

        private void Encerrar()
        {

            Console.WriteLine("Encerrando....");
            Thread.Sleep(200);
            return;

        }
    }
}
