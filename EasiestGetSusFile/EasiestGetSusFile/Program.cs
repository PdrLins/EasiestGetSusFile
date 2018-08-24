using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace EasiestGetSusFile
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("**************  BEM VINDO AO AVALIADOR DE VERSÃO TABELA UNIFICADA SIGTAP SUS  **************");
            Console.WriteLine("**************                  Desenvolvido por Pedro Lins                   **************");
            Console.ForegroundColor = ConsoleColor.Gray;
            var answer = "1";
            while (answer == "1")
            {
                try
                {
                    Console.WriteLine("Por favor digitar o path para ser salvo o arquivo: ");
                    var path = Console.ReadLine();
                    Console.WriteLine("Verificando Path...");
                    int milliseconds = 800;
                    Thread.Sleep(milliseconds);
                    if (Directory.Exists(path))
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Path Encontrado!!");
                        Thread.Sleep(600);
                        Console.WriteLine("Aguarde...");
                        Console.WriteLine($@"Arquivo {GetLatterVersion(path)}, salvo com sucesso ");
                        Console.WriteLine($@"Pressinoe ENTER para sair do programa.");
                        Console.ReadKey();
                        answer = "0";
                    }
                    else
                    {
                        throw new Exception("Path não encontrado");
                    }

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($@"Ocorreu uma exceção: {e.Message}");
                    Console.WriteLine("Desenja tentar novamente (1 = SIM, 0=NÃO) ?");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    answer = Console.ReadLine();
                }
            }
        }

        public static string GetLatterVersion(string path)
        {
            var finalVersion = string.Empty;
            string url = "ftp://ftp2.datasus.gov.br/pub/sistemas/tup/downloads";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential("anonymous", "pedro.lins@edax.com.br");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            List<FTPFileInfo> directories = new List<FTPFileInfo>();
            string line = streamReader.ReadLine();
            var a = new LoadFTP();

            while (!string.IsNullOrEmpty(line))
            {
                if (line.Contains(".zip"))
                {
                    directories.Add(a.Load(line));
                }

                line = streamReader.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"*********   {directories.Count} arquivos encontrados...   ******");
            Console.WriteLine($"Listando...");
            int milliseconds = 1000;
            Thread.Sleep(milliseconds);
            var i = 1;
            directories.ForEach(f =>
            {
                Thread.Sleep(150);
                Console.WriteLine($@" {i}: {f.FileName} {f.FileDate.ToShortDateString()} ");
                i++;
            });
            var latterVersion = directories.Last();
            Console.WriteLine($"****************");
            Console.WriteLine($"VERSÃO MAIS ATUAL :{latterVersion.FileName}");
            Console.WriteLine($"****************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"DESEJA REALIZAR O DOWNLOAD? (1=SIM, 2=NÃO)");
            streamReader.Close();
            request.Abort();
            var makedownlaod = Console.ReadLine();


            if (makedownlaod == "1")
            {

                try
                {
                    finalVersion = $@"{path}\{ latterVersion.FileName}";
                    WebClient request2 = new WebClient
                    {
                        Credentials = new NetworkCredential("anonymous", "contato@xxxx.com.br")
                    };
                    byte[] newFileData = request2.DownloadData($@"{url}\{latterVersion.FileName}");
                    String myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    File.WriteAllBytes(finalVersion, newFileData);
                    request2.Dispose();
                }
                catch (WebException e)
                {
                    throw e;
                }
                return finalVersion;
            }
            else
            {
                throw new Exception("Download não executado");
            }

        }

        public class FTPFileInfo
        {

            public DateTime FileDate
            {
                get;
                set;
            }
            public long FileSize
            {
                get;
                set;
            }
            public string FileName
            {
                get;
                set;
            }
        }
        public class LoadFTP
        {


            public FTPFileInfo LoadFromLine(string line)
            {
                FTPFileInfo file = new FTPFileInfo();

                string[] ftpFileInfo = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                file.FileDate = DateTime.Parse(ftpFileInfo[5] + " " + ftpFileInfo[6] + " " + ftpFileInfo[7]);
                file.FileSize = long.Parse(ftpFileInfo[4]);
                file.FileName = ftpFileInfo[8];

                return file;

            }
            public FTPFileInfo Load(string listDirectoryDetails)
            {
                var file = new FTPFileInfo();

                string[] lines = listDirectoryDetails.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return lines.Length == 1 ? LoadFromLine(lines[0]) : null;
            }

        }
    }
}
