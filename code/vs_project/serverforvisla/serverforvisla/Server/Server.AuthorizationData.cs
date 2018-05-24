using System;

namespace RemoteControl
{
    partial class Server
    {
        static void AuthorizationData(string prefix)
        {
            Console.Write(prefix + "Restoring user passwords data...");
            passwords.Add("god", "322SysAdmin");
            Console.Write(" done.\n" + prefix + "Adding usernames to tokens dictionary and access levels...");
            acceses.Add("god", 5);
            tokens.Add("god", null);
            Console.WriteLine(" done.");
        }
    }
}
