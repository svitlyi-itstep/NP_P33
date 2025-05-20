// CLIENT
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class UDPClientApp
{
    static int port = 5055;
    static string host = "127.0.0.1";

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.InputEncoding = UTF8Encoding.UTF8;

        UdpClient client = new UdpClient();
        IPEndPoint serverEP = new IPEndPoint(
            IPAddress.Parse(host), 
            port
        );

        string message = "Hello!";
        byte[] data = Encoding.UTF8.GetBytes(message);

        Console.WriteLine("Відправлення повідомлення...");
        client.Send(data, serverEP);
        Console.WriteLine("Повідомлення успішно надіслано!");

        byte[] response = client.Receive(ref serverEP);
        Console.WriteLine($"Відповідь від сервера: " +
            $"{Encoding.UTF8.GetString(response)}");
        Console.ReadLine();
    }
}
/*
    КЛІЄНТСЬКА ЧАСТИНА
    При запуску програми виводити меню, де дати можливість користувачу
        підключитися до чату. Клієнт має відправити на сервер повідомлення,
        яке буде вказувати серверу на бажання підключитися до чату (наприклад,
        "chat")
    Далі користувач має ввести свій username, під яким будуть розсилатися 
        повідомлення. Username також має відправитись на сервер. Все подальше
        введення користувача має сприйматися як повідомлення.
    В окремому потоці реалізувати прийом та виведення повідомлень від сервера.
*/
 
