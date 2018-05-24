using System;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] HTTPMainPageRequest(string prefix)
        {
            string reply = "HTTP/1.1 200 OK\nContent-Type: text/html; charset=utf-8;\n\n" + FAVICON + "<form method=\"post\" action=\"/login\">Username: <input name=\"username\"/><br/>Password: <input type=\"password\" name=\"password\"/><br/><input type=\"submit\" value=\"Log in!\"/></form>" + COPYRIGHT;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n{0}200 OK", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
    }
}
