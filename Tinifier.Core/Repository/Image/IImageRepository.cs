using System.Collections.Generic;
using Tinifier.Core.Models.Db;
using static Tinifier.Core.Repository.Image.TImageRepository;

namespace Tinifier.Core.Repository.Image
{
    /// <summary>
    /// Repository for images with custom Methods
    /// </summary>
    /// <typeparam name="Media">Media type</typeparam>
    public interface IImageRepository<Media>
    {
        void Update(int imageId, int actualSize);

        IEnumerable<Media> GetOptimizedItems();

        IEnumerable<TImage> GetTopOptimizedImages();

        IEnumerable<Media> GetItemsFromFolder(int folderId);

        int AmounthOfItems();

        int AmounthOfOptimizedItems();

        Media Get(string path);
    }
}
