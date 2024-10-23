using System.Security.Cryptography;
using System.Text;

namespace SLA.Domain.Application.Extensions
{
    public static class StringExtension
    {
        public static string CriptToMD5(this string value)
        {
            MD5 md5Hash = MD5.Create();
            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static bool Valid(this string value) 
        {
            if (string.IsNullOrEmpty(value)) return false;
            else return true;
        }
    }
}
