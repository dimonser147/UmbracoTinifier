using System.Collections.Generic;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IImageService
    {
        // Get all images from Umbraco Media
        IEnumerable<TImage> GetAllImages();

        // Get Image By Id from Umbraco Media
        TImage GetImageById(int id);

        // Update Image by tiny image
        void UpdateImage(TImage image, byte[] bytesArray);

        // Check Image Extension
        void CheckExtension(string source);

        // Get all optimized images
        IEnumerable<TImage> GetAllOptimizedImages();
    }
}
