using System;
using System.Diagnostics;

namespace RemoteControl
{
    partial class Server
    {
        static void LaunchCmd(string prefix)
        {
            Console.Write("{0}Launching cmd for rce...", prefix);
            cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    totalcmdoutput.Append("\n[" + cmdouputlines++ + "]: " + e.Data);
                }
            });
            cmd.Start();
            cmd.BeginOutputReadLine();
            Console.WriteLine(" done.");
        }
    }
}
