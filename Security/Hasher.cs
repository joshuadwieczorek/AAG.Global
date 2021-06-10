using System;
using System.Text;
using System.Security.Cryptography;

namespace AAG.Global.Security
{
    public class Hasher : IDisposable
    {
        /// <summary>
        /// MD5 Cryptography hasher.
        /// </summary>
        private static MD5 _md5;
        private static MD5 md5
        { 
            get
            {
                if (_md5 is null)
                    _md5 = MD5.Create();
                return _md5;
            }
        }


        /// <summary>
        /// Generate MD5 hash.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashMD5(string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Garbage cleanup.
        /// </summary>
        public void Dispose()
        {
            md5?.Dispose();
        }
    }
}