using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace RemoteControl
{
    partial class Server
    {
        #region WIN_API
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        #endregion WIN_API

        static Socket Listner;
        
        static Dictionary<string, string> passwords = new Dictionary<string, string>();
        static Dictionary<string, string> tokens = new Dictionary<string, string>();
        static Dictionary<string, int> acceses = new Dictionary<string, int>();
        static int currentlevel;

        static Process cmd;

        private static StringBuilder totalcmdoutput = new StringBuilder();
        static int cmdouputlines = 0;

        static StreamWriter sw = new StreamWriter("dump.txt");
    }
}
