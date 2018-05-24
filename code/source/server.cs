#define CONSOLE_REDIRECT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


class Server
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
    const int TIME_LIMIT = 1000;
	const string FAVICON = "<link rel=\"shortcut icon\" href=\"favicon.ico\" type=\"image/x-icon\">";
	const string COPYRIGHT = "<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>";

	static Dictionary<string,string> passwords = new Dictionary<string,string>();
	static Dictionary<string,string> tokens = new Dictionary<string,string>();
	static Dictionary<string,int> acceses = new Dictionary<string,int>();
	static int currentlevel;

	static Process cmd;

	private static StringBuilder totalcmdoutput = new StringBuilder();
	static int cmdouputlines = 0;

	static StreamWriter sw = new StreamWriter("dump.txt");

	public static void Main(string[] args)
	{
		Console.ForegroundColor = ConsoleColor.White;
		IPAddress ipAddress;
		if(args.Length == 0)
		{
			Console.Write("IP Address: ");
			ipAddress = IPAddress.Parse(Console.ReadLine());
		}
		else
		{
			ipAddress = IPAddress.Parse(args[0]);
		}
		var handle = GetConsoleWindow();
		ShowWindow(handle, SW_HIDE);
    	#if CONSOLE_REDIRECT
        Console.SetOut(sw);
    	#endif
		server_restart:
		try
		{
			AuthorizationData("");
			LaunchCmd("");
			Bind(ipAddress);
			Listen(""," ");
			Console.ReadKey();
		} catch { goto server_restart; }
	}
	static void LaunchCmd(string prefix)
	{
		Console.Write("{0}Launching cmd for rce...",prefix);
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
	static void AuthorizationData(string prefix)
	{
		Console.Write(prefix+"Restoring user passwords data...");
		passwords.Add("god","322SysAdmin");
		Console.Write(" done.\n"+prefix+"Adding usernames to tokens dictionary and access levels...");
		acceses.Add("god",5);
		tokens.Add("god",null);
		Console.WriteLine(" done.");
	}
	static void Bind(IPAddress ip, string prefix = "")
	{
		Console.WriteLine(prefix+"Start binding {0} at port 80",ip);
		Listner = new Socket(ip.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
		IPEndPoint endp = new IPEndPoint(ip,80);
		Listner.Bind(endp);
		Console.WriteLine(prefix+"Binded.");
	}
	static void Listen(string prefix = "", string secondprefix = "")
	{
		Console.WriteLine(prefix+"Start listening connections.");
		Listner.Listen(1);
		while(true)
		{
        #if CONSOLE_REDIRECT
			sw.Close();
			sw = new StreamWriter(new FileStream("dump.txt",FileMode.Append,FileAccess.Write));
			Console.SetOut(sw);
        #endif
			Console.Write(prefix+secondprefix+"Awaiting client...");
			Socket handler = Listner.Accept();
			Console.Write(" {0} connected.\n"+prefix+secondprefix+"Receiving data...",handler.LocalEndPoint);
			string data = null;
			byte[] bytes = new byte[67108864];
			int recievedLength = handler.Receive(bytes);
			data = Encoding.UTF8.GetString(bytes,0,recievedLength);
			Console.WriteLine(" data recieved.");
			byte[] msg;
			try
			{
				msg = HandleData(data,prefix+secondprefix+" "," ");
			}
			catch(Exception ex)
			{
				msg = Encoding.UTF8.GetBytes("HTTP/1.1 500 Internal Server Error\nContent-Type: text/html; cahrset=utf-8;\n\n"+FAVICON+"<h1 align=\"center\">500 Internal Server Error</h1><p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(prefix + ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
			}
			Console.WriteLine(prefix+secondprefix+"Sending data...");
			handler.Send(msg);
			Console.WriteLine(" done.\n"+prefix+secondprefix+"Shutting down connection.");
			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		}
	}
	static string HTMLCoder(string a)
	{
		string ret = "";
		for(int i = 0; i < a.Length; i++)
		{
			if(a[i]>='à'&&a[i]<='ÿ')
			{
				ret+="&#"+(a[i]-'à'+1072)+";";
			}
			else if(a[i]>='À'&&a[i]<='ß')
			{
				ret+="&#"+(a[i]-'À'+1040)+";";
			}
			else ret+=a[i];
		}
		return ret;
	}
	static byte[] HandleData(string data, string prefix = "", string secondprefix="")
	{
		currentlevel  = 0;
		Console.WriteLine(prefix+"Request parsing started.");
		Console.Write(prefix+secondprefix+"Parsing top string...");
		string[] lines = data.Split('\n');
		string[] __top = lines[0].Split(' ');
		string method = __top[0];
		string protocol = __top[__top.Length-1];
		string url = lines[0].Substring(method.Length+2,lines[0].Length-protocol.Length-method.Length-3);
		url = Uri.UnescapeDataString(url);
		Console.Write(" done.\n{0}{1}Parsing Headers...",prefix,secondprefix);
		Dictionary<string,string> Headers = new Dictionary<string,string>();
		int i = 1;
		for(; lines[i].Trim('\r')!=""; i++)
		{
			string name = lines[i].Split(':')[0];
			Headers.Add(name,lines[i].Substring(name.Length+1).Trim(' '));
		}
		Console.Write(" done.\n{0}{1}{2} requested",prefix,secondprefix,url);
		i++;
		string body = "";
		for(; i < lines.Length; i++)
		{
			body+=lines[i]+"\n";
		}
        if (url.ToLower().Contains("system32")) return Encoding.UTF8.GetBytes("HTTP/1.1 100 Continue\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">100 Continue</h1>"+RandomIWantMore()+"<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>");
		string[] parts = url.Split('/');
		if(url.EndsWith("favicon.ico"))
		{
			return HTTPFaviconRequest(prefix+secondprefix);
		}
		if(parts[0]=="")
			return HTTPMainPageRequest(prefix+secondprefix);
		if(parts[0]=="login")
		{
			if(method!="POST") return Error400(prefix);
			return PostAuth(body,prefix+secondprefix);
		}
		if(!HeaderAuth(Headers,prefix+secondprefix))
		{
			return Error423(prefix+secondprefix);
		}
		switch(parts[0])
		{
			case "files":
				if(currentlevel<1) return Error426(prefix+secondprefix); else return REQFiles(url.Substring(parts[0].Length+1),prefix+secondprefix);
			case "rce":
				if(currentlevel<5) return Error426(prefix+secondprefix); else return RemoteCommandExecution(url.Substring(parts[0].Length),body,prefix+secondprefix);
		}
		return Error404(prefix);
	}
	static byte[] PostAuth(string body, string prefix)
	{
		Console.Write("{0}Authorizing user...",prefix);
		string[] vars = body.Trim('\r').Trim(' ').Trim('\n').Split('&');
		Dictionary<string,string> names = new Dictionary<string,string>();
		for(int i = 0; i < vars.Length; i++)
		{
			string[] kvp = vars[i].Split('=');
			names.Add(kvp[0],vars[i].Substring(kvp[0].Length+1));
		}
		var q = from i in passwords where i.Key == names["username"] && i.Value == names["password"] select i;
		foreach(var a in q)
		{
			string t = GenRandomToken();
			tokens[names["username"]] = t;
			Console.WriteLine("succes.\n{0}{1} succesfully authorized",prefix,names["username"]);
			string reply = "HTTP/1.1 202 Accepted\nSet-Cookie: token="+t+"\nLocation: /files/\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">202 Accepted</h1>";
        	Console.ForegroundColor = ConsoleColor.Green;
        	Console.WriteLine("\n{0}202 Accepted", prefix);
        	Console.ForegroundColor = ConsoleColor.White;
        	return Encoding.UTF8.GetBytes(reply);
		}
		return Error423(prefix);
	}
	static bool HeaderAuth(Dictionary<string,string> headers, string prefix)
	{
		Console.Write("{0}Extracting Cookies from headers...",prefix);
		Dictionary<string,string> Cookies = new Dictionary<string,string>();
		if(!headers.ContainsKey("Cookie")) return false;
		string[] c = headers["Cookie"].Split(';');
		foreach(var cook in c)
		{
			string[] kvp = cook.Trim(' ').Split('=');
			Cookies.Add(kvp[0],cook.Substring(kvp[0].Length+1).Trim(' '));
		}
		Console.Write(" done.\n{0}Trying to auth...",prefix);
		foreach(var i in tokens)
		{
			if(i.Value == null) continue;
            bool ok = true;
			Cookies["token"] = Cookies["token"].Trim('\n').Trim('\r').Trim(' ');
            for (int j = 0; j < i.Value.Length; j++ )
            {
                if (i.Value[j] == Cookies["token"][j]) continue;
                else 
				{
					Console.Write(i.Value[j]+""+Cookies["token"][j]); ok =false;
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
	static byte[] HTTPFileRequest(string url,string prefix)
	{
			Console.Write("{0}File content to answer...",prefix);
			try
			{
				FileInfo fi  = new FileInfo(url);
				BinaryReader br = new BinaryReader(new FileStream(url,FileMode.Open,FileAccess.Read));
				byte[] msg = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\n\n");
				byte[] file = br.ReadBytes((int)fi.Length);
				byte[] ans = new byte[msg.Length+file.Length];
				msg.CopyTo(ans,0);
				file.CopyTo(ans,msg.Length);
				br.Close();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\n{0}200 OK",prefix);
				Console.ForegroundColor = ConsoleColor.White;
				return ans;
			}
			catch
			{
                return Error404(prefix);
			}
	}
	static byte[] HTTPDirectoryRequest(string url, string prefix)
	{
			DirectoryInfo dir = new DirectoryInfo(url);
			string reply = "";
			if(!dir.Exists)
			{
                return Error404(prefix);
			}
			DirectoryInfo[] directories = dir.GetDirectories();
			FileInfo[] files = dir.GetFiles();
			reply = "HTTP/1.1 200 OK\n\n";
			reply += "<!DOCTYPE html>"+FAVICON+"<h1 align=\"center\">Content of "+dir.FullName+"</h1>";
            Stopwatch sw = new Stopwatch();
            sw.Start();
			foreach(DirectoryInfo cdir in directories)
			{
                if (sw.ElapsedMilliseconds > TIME_LIMIT) break;
				reply+="<br/><a href=\"/files/"+Uri.EscapeUriString(cdir.FullName)+"\">"+cdir.Name+"</a>";
			}
			foreach(FileInfo cfile in files)
			{
                if (sw.ElapsedMilliseconds > TIME_LIMIT) break;
				reply+="<br/><a href=\"/files/"+Uri.EscapeUriString(cfile.FullName)+"\">"+cfile.Name+"</a>";
			}
            sw.Stop();
			reply += "<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>";
			reply = HTMLCoder(reply);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\n{0}200 OK",prefix);
			Console.ForegroundColor = ConsoleColor.White;
			return Encoding.UTF8.GetBytes(reply);
	}
	static byte[] HTTPFaviconRequest(string prefix)
	{
		FileInfo fi  = new FileInfo("fav.ico");
		BinaryReader br = new BinaryReader(new FileStream("fav.ico",FileMode.Open,FileAccess.Read));
		byte[] msg = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\nContent-Type: charset=UTF-8\n\n");
		byte[] file = br.ReadBytes((int)fi.Length);
		byte[] ans = new byte[msg.Length+file.Length];
		msg.CopyTo(ans,0);
		file.CopyTo(ans,msg.Length);
		br.Close();
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("\n{0}200 OK",prefix);
		Console.ForegroundColor = ConsoleColor.White;
		return ans;
	}
	static byte[] HTTPMainPageRequest(string prefix)
	{
		string reply = "HTTP/1.1 200 OK\nContent-Type: text/html; charset=utf-8;\n\n"+FAVICON+"<form method=\"post\" action=\"/login\">Username: <input name=\"username\"/><br/>Password: <input type=\"password\" name=\"password\"/><br/><input type=\"submit\" value=\"Log in!\"/></form>"+COPYRIGHT;
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("\n{0}200 OK",prefix);
		Console.ForegroundColor = ConsoleColor.White;
		return Encoding.UTF8.GetBytes(reply);
	}
	static byte[] REQFiles(string url, string prefix)
	{
		bool IsDirectory = Directory.Exists(url);
        if (!IsDirectory && !File.Exists(url))
        {
            return Error404(prefix);
        }
		if(!IsDirectory)
		{
			return HTTPFileRequest(url,prefix);
		}
		else
		{
			return HTTPDirectoryRequest(url,prefix);
		}
	}
	static byte[] RemoteCommandExecution(string url, string body, string prefix)
	{
		body = Uri.UnescapeDataString(body.Replace('+',' '));
		Console.WriteLine("{0}RCE Started",prefix);
		if(url=="")
		{
			string reply = "HTTP/1.1 200 OK\nContent-Type: text/html; charset=utf-8;\n\n<form method=\"POST\" action=\"rce/exec\"><input name=\"command\"/><input type=\"submit\"></input></form>"+COPYRIGHT;
			return Encoding.UTF8.GetBytes(reply);
		}
		else if(url=="/exec")
		{
			string[] vars = body.Trim('\r').Trim(' ').Trim('\n').Split('&');
			Dictionary<string,string> names = new Dictionary<string,string>();
			for(int i = 0; i < vars.Length; i++)
			{
				string[] kvp = vars[i].Split('=');
				names.Add(kvp[0],vars[i].Substring(kvp[0].Length+1));
			}
			Console.Write("{0}Executing command {1}...",prefix,names["command"]);
			cmd.StandardInput.WriteLine(names["command"]);
			string reply = "HTTP/1.1 202 Accepted\nRefresh: 0;/rce\nContent-Type: text/html;charset = utf-8;\n\n";
			Console.WriteLine(" done.");
			return Encoding.UTF8.GetBytes(reply);
		}
		else if(url=="/output")
		{
			string reply = "HTTP/1.1 200 OK\nContent-Type: text/plain; charset=utf-8;\nRefresh: 1;\n\n"+totalcmdoutput.ToString();
			return Encoding.UTF8.GetBytes(reply);
		}
		else return Error404(prefix);
	}
	static byte[] Error404(string prefix)
	{
		string reply = "HTTP/1.1 404 Not Found\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">404 Not Found</h1>";
        reply += "<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n{0}404 Not Found", prefix);
        Console.ForegroundColor = ConsoleColor.White;
        return Encoding.UTF8.GetBytes(reply);
	}
	static byte[] Error400(string prefix)
	{
		string reply = "HTTP/1.1 400 Bad Request\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">400 Bad Request</h1>";
        reply += "<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n{0}400 Bad Request", prefix);
        Console.ForegroundColor = ConsoleColor.White;
        return Encoding.UTF8.GetBytes(reply);
	}
	static byte[] Error423(string prefix)
	{
		string reply = "HTTP/1.1 423 Locked\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">423 Locked</h1>";
        reply += "<p style=\"font-size:8\" align=\"center\">&#169; SGH remote control 2016-2147483647</p>";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n{0}423 Locked", prefix);
        Console.ForegroundColor = ConsoleColor.White;
        return Encoding.UTF8.GetBytes(reply);
	}
	static byte[] Error426(string prefix)
	{
		string reply = "HTTP/1.1 426 Upgrade Required\nContent-Type: text/html; cahrset=utf-8;\n\n<h1 align=\"center\">426 Upgrade Required</h1>";
        reply += COPYRIGHT;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n{0}426 Upgrade Required", prefix);
        Console.ForegroundColor = ConsoleColor.White;
        return Encoding.UTF8.GetBytes(reply);
	}
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
	static string GenRandomToken()
	{
		Random r = new Random();
		string ans = "";
		for(int i = 0; i < 256; i++)
		{
			char c = (char)(r.Next(32,127));
			if(c==';')
			{
				i--; continue;
			}
			ans+=c;
		}
		return ans;
	}
}