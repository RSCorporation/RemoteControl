using System.IO;

namespace RemoteControl
{
    partial class Server
    {
        static byte[] REQFiles(string url, string prefix)
        {
            bool IsDirectory = Directory.Exists(url);
            if (!IsDirectory && !File.Exists(url))
            {
                return Error404(prefix);
            }
            if (!IsDirectory)
            {
                return HTTPFileRequest(url, prefix);
            }
            else
            {
                return HTTPDirectoryRequest(url, prefix);
            }
        }
    }
}
