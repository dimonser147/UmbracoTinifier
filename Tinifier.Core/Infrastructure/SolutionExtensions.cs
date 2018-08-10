using System;
using System.Text;

namespace Tinifier.Core.Infrastructure
{
    public static class SolutionExtensions
    {
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            if (int.TryParse(plainText, out var number))
                return plainText;

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
