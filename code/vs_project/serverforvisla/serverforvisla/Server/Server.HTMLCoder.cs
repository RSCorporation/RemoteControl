namespace RemoteControl
{
    partial class Server
    {
        static string HTMLCoder(string a)
        {
            string ret = "";
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] >= '�' && a[i] <= '�')
                {
                    ret += "&#" + (a[i] - '�' + 1072) + ";";
                }
                else if (a[i] >= '�' && a[i] <= '�')
                {
                    ret += "&#" + (a[i] - '�' + 1040) + ";";
                }
                else ret += a[i];
            }
            return ret;
        }
    }
}
