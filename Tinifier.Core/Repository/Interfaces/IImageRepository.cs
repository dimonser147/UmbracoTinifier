using System.Collections.Generic;

namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for images with custom Methods
    public interface IImageRepository<Media>
    {
        void UpdateItem(int imageId);

        IEnumerable<Media> GetOptimizedItems();

        IEnumerable<Media> GetItemsFromFolder(int folderId);

        int AmounthOfItems();

        int AmounthOfOptimizedItems();
    }
}
