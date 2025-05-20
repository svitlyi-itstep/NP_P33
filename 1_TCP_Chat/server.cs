// SERVER

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Text.Json;

class Message
{
    public string User { get; set; }
    public string Text { get; set; }
}

class Server
{
    static int port = 5050;
    static TcpListener listener;
    static List<TcpClient> clients = new List<TcpClient>();
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine("Сервер запущено.");
        Console.WriteLine("Очікування підключень...");
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static string GetFromStream(NetworkStream stream, int bufferSize=1024)
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

    static void HandleClient(object? obj)
    {
        if (obj == null) return;
        TcpClient client = (TcpClient)obj;
        var endPoint = client.Client.RemoteEndPoint;
        lock(clients) clients.Add(client);

        NetworkStream stream = client.GetStream();
        string name = GetFromStream(stream);
        // Broadcast($"{name} ({endPoint}) підключився до сервера");
        Broadcast(new Message
        {
            User = "SERVER",
            Text = $"{name} ({endPoint}) підключився до сервера"
        });
        try
        {
            while (true)
            {
                string text = GetFromStream(stream);
                Message message = JsonSerializer.Deserialize<Message>(text);
                // Broadcast($"[{name}] {message}");
                Broadcast(message);
            }
        }
        catch (Exception ex)
        {
            Broadcast(new Message
            {
                User = "SERVER",
                Text = $"{name} ({endPoint}) відключився від сервера"
            });
        }
        finally
        {
            client.Close();
            lock (clients) clients.Remove(client);
        }
    }

    static void Broadcast(string message) 
    {
        Console.WriteLine(message);
        lock (clients) {
            foreach (var client in clients)
            {
                try { SendToStream(client.GetStream(), message);}
                catch { }
            }
        }
    }
    static void Broadcast(Message message)
    {
        Console.WriteLine($"[{message.User}] {message.Text}");
        string json = JsonSerializer.Serialize(message);
        lock (clients)
        {
            foreach (var client in clients)
            {
                try { SendToStream(client.GetStream(), json); }
                catch { }
            }
        }
    }
    /*
        Написати функцію Broadcast, яка віправляє повідомлення усім підключеним
        клієнтам. Повідомлення має передаватися у вигляді параметру. Наприклад:
        
            Broadcast("New user") - має відправити повідомлення "New user" усім
            підключеним клієнтам.

        Для роботи функції потрібно окремо зберігати список підключених клієнтів.
    */
}
