using System;
using System.Net;
using System.Net.Sockets;

namespace RemoteControl
{
    partial class Server
    {
        static void Bind(IPAddress ip, string prefix)
        {
            Console.WriteLine(prefix + "Start binding {0} at port 80", ip);
            Listner = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endp = new IPEndPoint(ip, 80);
            Listner.Bind(endp);
            Console.WriteLine(prefix + "Binded.");
        }
    }
}
