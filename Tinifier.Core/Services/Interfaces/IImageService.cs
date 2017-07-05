using System.Collections.Generic;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IImageService
    {
        // Get Image By Id from Umbraco Media
        TImage GetImageById(int id);

        // Get all optimized images
        IEnumerable<TImage> GetAllOptimizedImages();

        // Get all images from specific folder
        IEnumerable<TImage> GetImagesFromFolder(int folderId);

        // Get image by path
        TImage GetImageByPath(string path);

        // Optimize image and update history
        void OptimizeImage(TImage image);

        // Update image and state if its image from folder
        void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, SourceTypes sourceType);
    }
}
