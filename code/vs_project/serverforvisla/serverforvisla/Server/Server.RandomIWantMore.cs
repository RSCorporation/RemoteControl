using System;

namespace RemoteControl
{
    partial class Server
    {
        static string RandomIWantMore()
        {
            string reply = "<p>";
            Random r = new Random();
            switch (r.Next(0, 5))
            {
                case 0: reply += "Yeah"; break;
                case 1: reply += "I want more!"; break;
                case 2: reply += "Continue, please..."; break;
                case 3: reply += "Dont stop!"; break;
            }
            return reply + "</p>";
        }
    }
}
