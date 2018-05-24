using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] PostAuth(string body, string prefix)
        {
            Console.Write("{0}Authorizing user...", prefix);
            string[] vars = body.Trim('\r').Trim(' ').Trim('\n').Split('&');
            Dictionary<string, string> names = new Dictionary<string, string>();
            for (int i = 0; i < vars.Length; i++)
            {
                string[] kvp = vars[i].Split('=');
                names.Add(kvp[0], vars[i].Substring(kvp[0].Length + 1));
            }
            var q = from i in passwords where i.Key == names["username"] && i.Value == names["password"] select i;
            foreach (var a in q)
            {
                string t = GenRandomToken();
                tokens[names["username"]] = t;
                Console.WriteLine("succes.\n{0}{1} succesfully authorized", prefix, names["username"]);
                string reply = "HTTP/1.1 202 Accepted\nSet-Cookie: token=" + t + "\nLocation: /files/\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">202 Accepted</h1>";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n{0}202 Accepted", prefix);
                Console.ForegroundColor = ConsoleColor.White;
                return Encoding.UTF8.GetBytes(reply);
            }
            return Error423(prefix);
        }
    }
}
