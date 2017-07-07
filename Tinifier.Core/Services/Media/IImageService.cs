using System.Collections.Generic;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Media
{
    public interface IImageService
    {
        /// <summary>
        /// Get Image By Id from Umbraco Media
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TImage GetImage(int id);

        /// <summary>
        /// Get image by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        TImage GetImage(string path);

        /// <summary>
        /// Get all optimized images
        /// </summary>
        /// <returns></returns>
        IEnumerable<TImage> GetOptimizedImages();

        /// <summary>
        /// Get all images from specific folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        IEnumerable<TImage> GetFolderImages(int folderId);

        /// <summary>
        /// Optimize image and update history
        /// </summary>
        /// <param name="image"></param>
        void OptimizeImage(TImage image);

        /// <summary>
        ///  Update image and state if its image from folder
        /// </summary>
        /// <param name="tinyResponse"></param>
        /// <param name="image"></param>
        /// <param name="sourceType"></param>
        void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, SourceTypes sourceType);
    }
}
