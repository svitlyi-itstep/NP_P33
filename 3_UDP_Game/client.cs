// CLIENT

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Player
{
    public int X { get; set; }
    public int Y { get; set; }

    public Player(int x, int y)
    {
        (X, Y) = (x, y);
    }
    public Player():this(0,0) { }
}

class Location
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public Location() 
    {
        (X, Y) = (0, 0);
        (Width, Height) = (20, 20);
    }
    public void Draw()
    {
        for(int y= 0; y < Height; y++)
        {
            Console.SetCursorPosition(X, Y + y);
            for(int x=0; x < Width; x++)
            {
                Player? player = GetPlayerByPosition(x, y);
                if(player == null) Console.Write("  ");
                else Console.Write("[]");
            }
        }
    }
    public Player? GetPlayerByPosition(int x, int y)
    {
        Player[] players;
        lock (Players) players = Players.ToArray();
        foreach (Player player in players)
            if (player.X == x && player.Y == y)
                return player;
        return null;
    }
}

class UDPClientApp
{
    static Player player = new Player();
    static Location location = new Location();
    static string serverIP = "127.0.0.1";
    static int serverPort = 5055;
    //static IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
    //static UdpClient server = new UdpClient();

    static void UpdateGame()
    {
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        UdpClient server = new UdpClient();
        while (true)
        {
            server.Send(Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(player)
            ), serverEP);

            Player[]? response =
                JsonSerializer.Deserialize<Player[]>(
                    Encoding.UTF8.GetString(
                        server.Receive(ref serverEP)
                    ));
            if (response != null)
            {
                lock (location.Players)
                {
                    location.Players.Clear();
                    location.Players.Add(player);
                    location.Players.AddRange(response);
                }
            }
            Thread.Sleep(10);
        }
    }

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        location.Players.Add(player);

        Thread updateThread = new Thread(UpdateGame);
        updateThread.Start();

        while(true)
        {
            if(Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow: player.X -= 1; break;
                    case ConsoleKey.RightArrow: player.X += 1; break;
                    case ConsoleKey.UpArrow: player.Y -= 1; break;
                    case ConsoleKey.DownArrow: player.Y += 1; break;
                }
            }
            location.Draw();
            Thread.Sleep(10);
        }
    }
}

/*
    Написати функцію, яка в окремому потоці буде відправляти на сервер
    інформацію про локального гравця та отримувати відповідь з переліком
    всіх інших гравців на сервері.
 
    Також функція має оновлювати список гравців у об`єкті класу Location,
    щоб вони виводились на екран клієнта.
*/
