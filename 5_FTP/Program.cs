using System.Net;
using System.Text;


class FTPClient
{
    static string ftpHost = "ftp://ftpupload.net";
    static string username = "ftp-username";
    static string password = "ftp-password";

    static string[] FtpListDirectory(string path = "/")
    {
        try
        {
            string uri = ftpHost + path;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            List<string> elements = new List<string>();
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line != null) elements.Add(line);
            }
            return elements.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return new string[] { };
    }

    static void FtpDownloadFile(string remotePath, string? localPath = null)
    {
        try
        {
            if(localPath == null)
            {
                localPath = $"{Path.GetDirectoryName(Environment.ProcessPath)}/" +
                    $"{Path.GetFileName(remotePath)}";
            }
            string uri = ftpHost + remotePath;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream fs = new FileStream(localPath, FileMode.Create))
            {
                responseStream.CopyTo(fs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void FtpUploadFile(string localPath, string? remotePath = null)
    {
        try
        {
            if (remotePath == null)
            {
                remotePath = $"/{Path.GetFileName(localPath)}";
            }
            string uri = ftpHost + remotePath;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);

            byte[] fileContent = File.ReadAllBytes(localPath);
            request.ContentLength = fileContent.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContent, 0, fileContent.Length);
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        /*
        Console.WriteLine("Отримання файлів з серверу...");
        string[] filesList = FtpListDirectory();
        int fileIndex = 0;
        while(true)
        {
            Console.Clear();
            for(int i = 0; i < filesList.Length; i++)
            {
                if (i == fileIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.WriteLine(filesList[i]);
                Console.ResetColor();
            }
            switch(Console.ReadKey(true).Key) 
            {
                case ConsoleKey.UpArrow: { fileIndex--; break; }
                case ConsoleKey.DownArrow: { fileIndex++; break; }
            }
        }
        */
        Console.WriteLine("Завантаження файлу з сервера...");
        FtpDownloadFile("/data2.txt");
        Console.WriteLine("Завантаження файлу на сервер...");
        FtpUploadFile($"{Path.GetDirectoryName(Environment.ProcessPath)}/test.txt");
    }
}
