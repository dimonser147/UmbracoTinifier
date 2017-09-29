namespace Tinifier.Core.Services.TinyPNG
{
    public interface ITinyImageService
    {
        /// <summary>
        /// Download image from url
        /// </summary>
        /// <param name="url">Image url</param>
        /// <returns>byte[]</returns>
        byte[] DownloadImage(string url);
    }
}
