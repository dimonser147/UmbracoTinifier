namespace Tinifier.Core.Services.Interfaces
{
    public interface ITinyImageService
    {
        // Download image from url
        byte[] GetTinyImage(string url);
    }
}
