using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] RemoteCommandExecution(string url, string body, string prefix)
        {
            body = Uri.UnescapeDataString(body.Replace('+', ' '));
            Console.WriteLine("{0}RCE Started", prefix);
            if (url == "")
            {
                string reply = "HTTP/1.1 200 OK\nContent-Type: text/html; charset=utf-8;\n\n<form method=\"POST\" action=\"rce/exec\"><input name=\"command\"/><input type=\"submit\"></input></form>" + COPYRIGHT;
                return Encoding.UTF8.GetBytes(reply);
            }
            else if (url == "/exec")
            {
                string[] vars = body.Trim('\r').Trim(' ').Trim('\n').Split('&');
                Dictionary<string, string> names = new Dictionary<string, string>();
                for (int i = 0; i < vars.Length; i++)
                {
                    string[] kvp = vars[i].Split('=');
                    names.Add(kvp[0], vars[i].Substring(kvp[0].Length + 1));
                }
                Console.Write("{0}Executing command {1}...", prefix, names["command"]);
                cmd.StandardInput.WriteLine(names["command"]);
                string reply = "HTTP/1.1 202 Accepted\nRefresh: 0;/rce\nContent-Type: text/html;charset = utf-8;\n\n";
                Console.WriteLine(" done.");
                return Encoding.UTF8.GetBytes(reply);
            }
            else if (url == "/output")
            {
                string reply = "HTTP/1.1 200 OK\nContent-Type: text/plain; charset=utf-8;\nRefresh: 1;\n\n" + totalcmdoutput.ToString();
                return Encoding.UTF8.GetBytes(reply);
            }
            else return Error404(prefix);
        }
    }
}
