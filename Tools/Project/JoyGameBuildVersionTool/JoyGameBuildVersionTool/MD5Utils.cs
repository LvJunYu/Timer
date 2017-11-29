using System.Text;

namespace JoyGameBuildVersionTool
{
    public abstract class MD5Utils
    {
        public static string Encrypt(byte[] buffer, int len = 0)
        {
            if (len == 0)
            {
                len = buffer.Length;
            }
            System.Security.Cryptography.MD5 alg = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = alg.ComputeHash(buffer, 0, len);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string Encrypt(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }
            return Encrypt(Encoding.UTF8.GetBytes(s));
        }
    }
}