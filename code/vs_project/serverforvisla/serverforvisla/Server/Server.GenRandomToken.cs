using System;

namespace RemoteControl
{
    partial class Server
    {
        static string GenRandomToken()
        {
            Random r = new Random();
            string ans = "";
            for (int i = 0; i < 256; i++)
            {
                char c = (char)(r.Next(32, 127));
                if (c == ';')
                {
                    i--; continue;
                }
                ans += c;
            }
            return ans;
        }
    }
}
