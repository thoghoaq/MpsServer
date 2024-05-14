using System.Text;

namespace Mps.Infrastructure.Dependencies.VnPay.Helpers
{
    public class HashHelper
    {
        public static string HmacSHA512(string key, string inputData)
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
