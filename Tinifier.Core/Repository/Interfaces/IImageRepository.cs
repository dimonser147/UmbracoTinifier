using System.Collections.Generic;
using Umbraco.Core.Services;

namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for images with custom Methods
    public interface IImageRepository<Media>
    {
        void UpdateItem(IMediaService mediaService, Media mediaItem);

        IEnumerable<Media> GetOptimizedItems();

        IEnumerable<Media> GetItemsFromFolder(int folderId);

        int AmounthOfItems();

        int AmounthOfOptimizedItems();
    }
}
