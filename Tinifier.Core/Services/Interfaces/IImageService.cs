using System.Collections.Generic;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.API;
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

        // Get all optimized images
        IEnumerable<TImage> GetAllOptimizedImages();

        // Get all images from specific folder
        IEnumerable<TImage> GetImagesFromFolder(int folderId);

        string GetUrl(string path);

        void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, SourceTypes sourceType);
    }
}
