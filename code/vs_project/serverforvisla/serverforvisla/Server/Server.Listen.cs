using System;
using System.Net.Sockets;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static void Listen(string prefix, string secondprefix)
        {
            Console.WriteLine(prefix + "Start listening connections.");
            Listner.Listen(1);
            while (true)
            {
#if CONSOLE_REDIRECT
                sw.Close();
                sw = new StreamWriter(new FileStream("dump.txt", FileMode.Append, FileAccess.Write));
                Console.SetOut(sw);
#endif
                Console.Write(prefix + secondprefix + "Awaiting client...");
                Socket handler = Listner.Accept();
                Console.Write(" {0} connected.\n" + prefix + secondprefix + "Receiving data...", handler.LocalEndPoint);
                string data = null;
                byte[] bytes = new byte[67108864];
                int recievedLength = handler.Receive(bytes);
                data = Encoding.UTF8.GetString(bytes, 0, recievedLength);
                Console.WriteLine(" data recieved.");
                byte[] msg;
                try
                {
                    msg = HandleData(data, prefix + secondprefix + " ", " ");
                }
                catch (Exception ex)
                {
                    msg = Encoding.UTF8.GetBytes("HTTP/1.1 500 Internal Server Error\nContent-Type: text/html; cahrset=utf-8;\n\n" + FAVICON + "<h1 align=\"center\">500 Internal Server Error</h1>" + COPYRIGHT);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(prefix + ex.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(prefix + secondprefix + "Sending data...");
                handler.Send(msg);
                Console.WriteLine(" done.\n" + prefix + secondprefix + "Shutting down connection.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}
