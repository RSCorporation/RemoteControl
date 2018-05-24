using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] HTTPDirectoryRequest(string url, string prefix)
        {
            DirectoryInfo dir = new DirectoryInfo(url);
            string reply = "";
            if (!dir.Exists)
            {
                return Error404(prefix);
            }
            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            reply = "HTTP/1.1 200 OK\n\n";
            reply += "<!DOCTYPE html>" + FAVICON + "<h1 align=\"center\">Content of " + dir.FullName + "</h1>";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (DirectoryInfo cdir in directories)
            {
                if (sw.ElapsedMilliseconds > TIME_LIMIT) break;
                reply += "<br/><a href=\"/files/" + Uri.EscapeUriString(cdir.FullName) + "\">" + cdir.Name + "</a>";
            }
            foreach (FileInfo cfile in files)
            {
                if (sw.ElapsedMilliseconds > TIME_LIMIT) break;
                reply += "<br/><a href=\"/files/" + Uri.EscapeUriString(cfile.FullName) + "\">" + cfile.Name + "</a>";
            }
            sw.Stop();
            reply += COPYRIGHT;
            reply = HTMLCoder(reply);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n{0}200 OK", prefix);
            Console.ForegroundColor = ConsoleColor.White;
            return Encoding.UTF8.GetBytes(reply);
        }
    }
}
