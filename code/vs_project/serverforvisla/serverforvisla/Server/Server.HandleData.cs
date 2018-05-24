using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] HandleData(string data, string prefix = "", string secondprefix = "")
        {
            currentlevel = 0;
            Console.WriteLine(prefix + "Request parsing started.");
            Console.Write(prefix + secondprefix + "Parsing top string...");
            string[] lines = data.Split('\n');
            string[] __top = lines[0].Split(' ');
            string method = __top[0];
            string protocol = __top[__top.Length - 1];
            string url = lines[0].Substring(method.Length + 2, lines[0].Length - protocol.Length - method.Length - 3);
            url = Uri.UnescapeDataString(url);
            Console.Write(" done.\n{0}{1}Parsing Headers...", prefix, secondprefix);
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            int i = 1;
            for (; lines[i].Trim('\r') != ""; i++)
            {
                string name = lines[i].Split(':')[0];
                Headers.Add(name, lines[i].Substring(name.Length + 1).Trim(' '));
            }
            Console.Write(" done.\n{0}{1}{2} requested", prefix, secondprefix, url);
            i++;
            string body = "";
            for (; i < lines.Length; i++)
            {
                body += lines[i] + "\n";
            }
            if (url.ToLower().Contains("system32")) return Encoding.UTF8.GetBytes("HTTP/1.1 100 Continue\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">100 Continue</h1>" + RandomIWantMore() + COPYRIGHT);
            string[] parts = url.Split('/');
            if (url.EndsWith("favicon.ico"))
            {
                return HTTPFaviconRequest(prefix + secondprefix);
            }
            if (parts[0] == "")
                return HTTPMainPageRequest(prefix + secondprefix);
            if (parts[0] == "login")
            {
                if (method != "POST") return Error400(prefix);
                return PostAuth(body, prefix + secondprefix);
            }
            if (!HeaderAuth(Headers, prefix + secondprefix))
            {
                return Error423(prefix + secondprefix);
            }
            switch (parts[0])
            {
                case "files":
                    if (currentlevel < 1) return Error426(prefix + secondprefix); else return REQFiles(url.Substring(parts[0].Length + 1), prefix + secondprefix);
                case "rce":
                    if (currentlevel < 5) return Error426(prefix + secondprefix); else return RemoteCommandExecution(url.Substring(parts[0].Length), body, prefix + secondprefix);
            }
            return Error404(prefix);
        }
    }
}
