namespace Tinifier.Core.Interfaces
{
    public interface ITinyImageService
    {
        // Download image from url
        byte[] GetTinyImage(string url);
    }
}
