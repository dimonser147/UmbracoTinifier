using System;
using System.Collections.Generic;
using System.Linq;
using Tinifier.Core.Repository.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace Tinifier.Core.Repository.Repository
{
    public class TImageRepository : IEntityReader<Media>, IImageRepository<Media>
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaService _mediaService;
        private readonly UmbracoDatabase _database;

        public TImageRepository()
        {
            _mediaService = ApplicationContext.Current.Services.MediaService;
            _database = ApplicationContext.Current.DatabaseContext.Database;
            _contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        }

        public IEnumerable<Media> GetAll()
        {
            var mediaList = new List<Media>();
            var mediaItems = _mediaService.GetMediaOfMediaType(_contentTypeService.GetMediaType("image").Id);

            foreach(var item in mediaItems)
            {
                mediaList.Add(item as Media);
            }

            return mediaList;
        }

        public Media GetByKey(int id)
        {
            var mediaItem = _mediaService.GetById(id) as Media;

            return mediaItem;
        }

        public void UpdateItem(int imageId)
        {
            var mediaItem = _mediaService.GetById(imageId) as Media;

            mediaItem.UpdateDate = DateTime.UtcNow;

            _mediaService.Save(mediaItem);
        }

        public IEnumerable<Media> GetOptimizedItems()
        {
            var mediaList = new List<Media>();
            var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var historyIds = _database.Fetch<int>(query);

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType("image").Id).
                             Where(item => historyIds.Contains(item.Id));

            foreach (var item in mediaItems)
            {
                mediaList.Add(item as Media);
            }

            return mediaList;
        }

        public IEnumerable<Media> GetItemsFromFolder(int folderId)
        {
            var mediaList = new List<Media>();
            var imagesFolder = _mediaService.GetById(folderId);

            foreach(var media in imagesFolder.Children())
            {
                if (media.ContentType.Alias == "Image")
                {
                    mediaList.Add(media as Media);
                }
            }

            return mediaList;
        }

        public int AmounthOfItems()
        {
            var mediaItems = _mediaService.GetMediaOfMediaType(_contentTypeService.GetMediaType("image").Id);
            var numberOfItems = mediaItems.ToList().Count();

            return numberOfItems;
        }

        public int AmounthOfOptimizedItems()
        {
            var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var historyIds = _database.Fetch<int>(query);

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType("image").Id).
                             Where(item => historyIds.Contains(item.Id));

            var numberOfOptimizedItems = mediaItems.ToList().Count();

            return numberOfOptimizedItems;
        }

        public Media GetByPath(string path)
        {
            var mediaItem = _mediaService.GetMediaByPath(path) as Media;

            return mediaItem;
        }
    }
}
