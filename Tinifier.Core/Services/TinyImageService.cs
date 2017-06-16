using System.Net;
using Tinifier.Core.Interfaces;

namespace Tinifier.Core.Services
{
    public sealed class TinyImageService : ITinyImageService
    {
        private static readonly TinyImageService instance = new TinyImageService();

        static TinyImageService()
        {
        }

        public static TinyImageService Instance
        {
            get
            {
                return instance;
            }
        }

        public byte[] GetTinyImage(string url)
        {
            byte[] tinyImageBytes;

            using (var webClient = new WebClient())
            {
                tinyImageBytes = webClient.DownloadData(url);
            }

            return tinyImageBytes;
        }
    }
}
