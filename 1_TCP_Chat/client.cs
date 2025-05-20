// CLIENT

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Message
{
    public string User { get; set; }
    public string Text { get; set; }
}


class Client
{
    static string serverIP = "127.0.0.1";
    static int port = 5050;
    static string GetFromStream(NetworkStream stream, int bufferSize = 1024)
    {
        byte[] buffer = new byte[bufferSize];
        stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer).Split(char.MinValue).First();
    }

    static void SendToStream(NetworkStream stream, string message,
        int bufferSize = 1024)
    {
        byte[] buffer = new byte[bufferSize];
        buffer = Encoding.UTF8.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }

    static void GetServerOutput(object? obj)
    {
        if (obj == null) return;
        NetworkStream stream = (NetworkStream)obj;
        while (true)
        {
            try
            {
                Message message =
                    JsonSerializer.Deserialize<Message>(GetFromStream(stream));
                Console.WriteLine($"[{message.User}] {message.Text}");
            }
            catch { }
        }
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        Console.WriteLine("Підключення до сервера...");
        TcpClient client = new TcpClient(serverIP, port);
        Console.WriteLine("Успішно підключено до сервера.");

        Console.WriteLine("Введіть своє ім`я: ");
        string name = Console.ReadLine();

        NetworkStream stream = client.GetStream();
        SendToStream(stream, name);

        Thread serverOutputThread = new Thread(GetServerOutput);
        serverOutputThread.Start(stream);

        while(true) { 
            string text = Console.ReadLine();
            Message message = new Message
            {
                User = name,
                Text = text
            };
            SendToStream(stream, JsonSerializer.Serialize(message)); 
        }
        
        Console.WriteLine("Натисніть Enter, щоб вийти...");
        Console.ReadLine();
    }
}
