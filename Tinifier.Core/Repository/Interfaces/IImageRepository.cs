using System.Collections.Generic;
using Umbraco.Core.Services;

namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for images with custom Methods
    public interface IImageRepository<TEntity> where TEntity : class
    {
        void UpdateItem(IMediaService mediaService, TEntity mediaItem);

        IEnumerable<TEntity> GetOptimizedItems();
    }
}
