using System.Security.Cryptography;
using System.Text;

namespace Loans.Extensions
{
    public static class HashExtensions
    {
        private static readonly MD5 HashAlgorithm = MD5.Create();

        public static string Hash(this string value)
        {
            char ToHex(int x) => (char)(x < 10 ? x + '0' : x + ('a' - 10));

            byte[] hash = HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            char[] stringBuffer = new char[hash.Length * 2];

            for (int i = 0; i < hash.Length; ++i)
            {
                stringBuffer[2 * i + 0] = ToHex(hash[i] >> 4);
                stringBuffer[2 * i + 1] = ToHex(hash[i] & 0x0F);
            }

            return new string(stringBuffer);
        }
    }
}
