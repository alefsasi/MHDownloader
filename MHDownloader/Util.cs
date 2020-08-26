using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MHostDownlaoder
{
    public class Util
    {

        private static WebClient client;
        public  static string[] ReplaceList = { "Cap&iacute;tulo", "Ler Online - ", "[]", "?"};

        public static WebClient GetWebClient()
        {
          
            if(client == null)
            {
                client = new WebClient();
            }

            return client;
        }

        public static void CloseWebClient()
        {
            client.Dispose();
        }

        public static void CreateFolder(string folderName)
        {
            try
            {
                
                if (Directory.Exists(folderName))
                {
                    return;
                }
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(folderName);
            }
            catch (Exception)
            {
                throw new Exception("A criação da Pasta Falhou");
            }
        }

        public static void DownlaodImage(WebClient cliente, string mangaName, string chapterName, string imageName, string urlFile)
        {
           


            var path = $@"{mangaName.Replace("\\","")}/{chapterName}/";
            path = path.Replace(":", "");

            try
            {

                CreateFolder(path);

                var filepath = path + imageName.Replace(".webp", "");


                if (!File.Exists(filepath))
                {
                    cliente.DownloadFile(urlFile, filepath);

                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

        }
    }
}
