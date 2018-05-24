using System;
using System.Collections.Generic;

namespace RemoteControl
{
    partial class Server
    {
        static bool HeaderAuth(Dictionary<string, string> headers, string prefix)
        {
            Console.Write("{0}Extracting Cookies from headers...", prefix);
            Dictionary<string, string> Cookies = new Dictionary<string, string>();
            if (!headers.ContainsKey("Cookie")) return false;
            string[] c = headers["Cookie"].Split(';');
            foreach (var cook in c)
            {
                string[] kvp = cook.Trim(' ').Split('=');
                Cookies.Add(kvp[0], cook.Substring(kvp[0].Length + 1).Trim(' '));
            }
            Console.Write(" done.\n{0}Trying to auth...", prefix);
            foreach (var i in tokens)
            {
                if (i.Value == null) continue;
                bool ok = true;
                Cookies["token"] = Cookies["token"].Trim('\n').Trim('\r').Trim(' ');
                for (int j = 0; j < i.Value.Length; j++)
                {
                    if (i.Value[j] == Cookies["token"][j]) continue;
                    else
                    {
                        Console.Write(i.Value[j] + "" + Cookies["token"][j]); ok = false;
                    }
                }
                if (ok)
                {
                    currentlevel = acceses[i.Key];
                    Console.WriteLine(" success.\n{0}{1} authorized", prefix, i.Key);
                    return true;
                }
            }
            Console.WriteLine(" authorization failed.");
            return false;
        }
    }
}
