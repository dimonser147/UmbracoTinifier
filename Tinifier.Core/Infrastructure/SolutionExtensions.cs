using System;
using System.IO;
using System.Text;
using Umbraco.Core.Models;
using Umbraco.Web;

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

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get absolute url of Umbraco media file
        /// </summary>
        /// <param name="uMedia"></param>
        /// <returns></returns>
        public static string GetAbsoluteUrl(Media uMedia)
        {
            var umbHelper = new UmbracoHelper(UmbracoContext.Current);
            var content = umbHelper.Media(uMedia.Id);
            var imagerUrl = content.Url;
            return imagerUrl;
        }
    }
}
