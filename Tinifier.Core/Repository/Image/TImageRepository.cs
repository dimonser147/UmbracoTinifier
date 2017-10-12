using System;
using System.Collections.Generic;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Services;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace Tinifier.Core.Repository.Image
{
    public class TImageRepository : IEntityReader<Media>, IImageRepository<Media>
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaService _mediaService;
        private readonly UmbracoDatabase _database;
        private readonly List<Media> mediaList = new List<Media>();

        public TImageRepository()
        {
            _mediaService = ApplicationContext.Current.Services.MediaService;
            _database = ApplicationContext.Current.DatabaseContext.Database;
            _contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        }

        /// <summary>
        /// Get all media
        /// </summary>
        /// <returns>IEnumerable of Media</returns>
        public IEnumerable<Media> GetAll()
        {
            var mediaItems = _mediaService.GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id);

            return mediaItems.Select(item => item as Media).ToList();
        }

        /// <summary>
        /// Get Media by Id
        /// </summary>
        /// <param name="id">Media Id</param>
        /// <returns>Media</returns>
        public Media Get(int id)
        {
            return _mediaService.GetById(id) as Media;
        }

        /// <summary>
        /// Get Media By path
        /// </summary>
        /// <param name="path">relative path</param>
        /// <returns>Media</returns>
        public Media Get(string path)
        {
            return _mediaService.GetMediaByPath(path) as Media;
        }

        /// <summary>
        /// Update Media
        /// </summary>
        /// <param name="id">Media Id</param>
        public void Update(int id, int actualSize)
        {
            var mediaItem = _mediaService.GetById(id) as Media;

            if (mediaItem != null)
            {
                mediaItem.SetValue("umbracoBytes", actualSize);  
                mediaItem.UpdateDate = DateTime.UtcNow;
                // raiseEvents: false - #2827
                _mediaService.Save(mediaItem, raiseEvents: false);
            }
        }

        /// <summary>
        /// Get Optimized Images
        /// </summary>
        /// <returns>IEnumerable of Media</returns>
        public IEnumerable<Media> GetOptimizedItems()
        {
            var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var historyIds = _database.Fetch<int>(query);

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                             Where(item => historyIds.Contains(item.Id));

            return mediaItems.Select(item => item as Media).ToList();
        }

        /// <summary>
        /// Get Media from folder
        /// </summary>
        /// <param name="folderId">Folder Id</param>
        /// <returns>IEnumerable of Media</returns>
        public IEnumerable<Media> GetItemsFromFolder(int folderId)
        {
            var imagesFolder = _mediaService.GetById(folderId);
            var items = imagesFolder.Children();

            if (items.Any())
            {
                foreach (var media in items)
                {
                    if (media.ContentType.Alias.ToLower() == PackageConstants.ImageAlias)
                    {
                        mediaList.Add(media as Media);
                    }
                } 
                foreach(var media in items)
                {
                    if (media.ContentType.Alias.ToLower() == PackageConstants.FolderAlias)
                    {
                        GetItemsFromFolder(media.Id);
                    }
                }  
            }

            return mediaList;
        }

        /// <summary>
        /// Get Count of Images
        /// </summary>
        /// <returns>Number of Images</returns>
        public int AmounthOfItems()
        {
            var mediaItems = _mediaService.GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id);
            var numberOfItems = mediaItems.Count();

            return numberOfItems;
        }

        /// <summary>
        /// Get Count of Optimized Images
        /// </summary>
        /// <returns>Number of optimized Images</returns>
        public int AmounthOfOptimizedItems()
        {
            var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var historyIds = _database.Fetch<int>(query);

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                             Where(item => historyIds.Contains(item.Id));

            return mediaItems.Count();
        }

        public IEnumerable<Media> GetTopOptimizedImages()
        {
            var query = new Sql("SELECT ImageId, OriginSize, OptimizedSize FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var optimizedImages = _database.Fetch<TopImagesModel>(query);
            var historyIds = optimizedImages.OrderByDescending(x => (x.OriginSize - x.OptimizedSize)).Select(y => y.ImageId).Take(50);

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                             Where(item => historyIds.Contains(item.Id));

            return mediaItems.Select(item => item as Media).ToList();
        }
    }
}
