using System.Net;

namespace Tinifier.Core.Services.TinyPNG
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

        public byte[] DownloadImage(string url)
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
