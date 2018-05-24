using System;
using System.IO;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] HTTPFaviconRequest(string prefix)
        {
            FileInfo fi = new FileInfo("fav.ico");
            BinaryReader br = new BinaryReader(new FileStream("fav.ico", FileMode.Open, FileAccess.Read));
            byte[] msg = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\nContent-Type: charset=UTF-8\n\n");
            byte[] file = br.ReadBytes((int)fi.Length);
            byte[] ans = new byte[msg.Length + file.Length];
            msg.CopyTo(ans, 0);
            file.CopyTo(ans, msg.Length);
            br.Close();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n{0}200 OK", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return ans;
        }
    }
}
