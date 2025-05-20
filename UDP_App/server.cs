// SERVER

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class UDPServerApp
{
    static int port = 5055;
    static List<IPEndPoint> clients = new List<IPEndPoint>();
    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        UdpClient server = new UdpClient(port);
        Console.WriteLine("Сервер запущено!\nОчікування повідомлень...");
        while(true)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = server.Receive(ref remoteEP);
            string message = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Отримано повідомлення від {remoteEP}: {message}");

            server.Send(Encoding.UTF8.GetBytes("OK"), remoteEP);
        }
    }
}

/*
    СЕРВЕРНА ЧАСТИНА
    При отриманні повідомлення "chat" зберігати ендпоінт відправника як користувача
        чату. Наступне повідомлення від цього клієнта має сприйматися як його
        username. Всі подальші повідомлення сприймати як звичайні повідомлення
        та розсилати іншим користувачам чату.
*/
 
