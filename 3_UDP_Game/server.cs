// SERVER

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// Клас Player (такий самий, як у клієнті)
class Player
{
    public int X { get; set; }
    public int Y { get; set; }

    public Player(int x, int y)
    {
        (X, Y) = (x, y);
    }
    public Player() : this(0, 0) { }
}

class UDPServerApp
{
    static int port = 5055;
    static UdpClient server = new UdpClient(port);
    static Dictionary<IPEndPoint, Player> players = 
        new Dictionary<IPEndPoint, Player>();

    static Player[] GetResponseForClient(IPEndPoint client)
    {
        List<Player> response = new List<Player>();
        foreach(var player in players)
            if (!player.Key.Equals(client))
                response.Add(player.Value);
        return response.ToArray();
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;
        Console.WriteLine("Очікування на повідомлення...");
        while(true)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            // Отримання повідомлення від клієнта
            byte[] data = server.Receive(ref remoteEP);
            // Десеріалізація у клас Player
            Player? player = JsonSerializer.Deserialize<Player>(
                Encoding.UTF8.GetString(data)
            );
            if (player == null) continue;
            // Оновлення інформації про гравця на сервері
            if (players.ContainsKey(remoteEP)) players[remoteEP] = player;
            else players.Add(remoteEP, player);
            // Формування та відправка відповіді
            Player[] response = GetResponseForClient(remoteEP);
            server.Send(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)), 
                remoteEP
            );
            Console.WriteLine($"{remoteEP}: Player(X={player.X}, Y={player.Y}) {players.Keys.Count}");
            foreach(var pl in response) { Console.Write($"({pl.X}, {pl.Y})"); }
            Console.WriteLine();
        }
    }
}

/*
 
    Змінити код сервера таким чином, щоб він відстежував час останнього повідомлення від
    клієнта і у випадку довгої відсутності повідомлень відключав клієнт від серверу (має 
    видалятися запис про цей клієнт зі словника).

    Додати у клас Player поле, яке буде зберігати колір гравця (рекомендовано використати
    тип ConsoleColor). При підключенні нового гравця до сервера, сервер встановлює гравцю
    випадковий колір (який фіксується у об`єкті класу Player). Клієнт має виводити гравців
    їхнім кольором.

*/
 
