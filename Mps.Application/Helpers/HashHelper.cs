using System.Text;

namespace Mps.Application.Helpers
{
    public static class HashHelper
    {
        public static string HmacSHA512(this string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (byte x in hashValue)
                {
                    hash.AppendFormat(x.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}
