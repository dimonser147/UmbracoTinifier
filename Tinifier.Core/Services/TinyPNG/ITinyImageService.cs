namespace Tinifier.Core.Services.TinyPNG
{
    public interface ITinyImageService
    {
        // Download image from url
        byte[] GetTinyImage(string url);
    }
}
