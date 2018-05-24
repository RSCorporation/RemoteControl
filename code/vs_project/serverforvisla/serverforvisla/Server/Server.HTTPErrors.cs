#define CONSOLE_REDIRECT
#define MANUAL_INIT
#define ONLINE_NETWORK

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] Error404(string prefix)
        {
            string reply = "HTTP/1.1 404 Not Found\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">404 Not Found</h1>";
            reply += COPYRIGHT;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n{0}404 Not Found", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
        static byte[] Error400(string prefix)
        {
            string reply = "HTTP/1.1 400 Bad Request\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">400 Bad Request</h1>";
            reply += COPYRIGHT;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n{0}400 Bad Request", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
        static byte[] Error423(string prefix)
        {
            string reply = "HTTP/1.1 423 Locked\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">423 Locked</h1>";
            reply += COPYRIGHT;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n{0}423 Locked", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
        static byte[] Error426(string prefix)
        {
            string reply = "HTTP/1.1 426 Upgrade Required\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">426 Upgrade Required</h1>";
            reply += COPYRIGHT;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n{0}426 Upgrade Required", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
    }
}