using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Models.Services;
using Tinifier.Core.Repository.Common;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

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
        /// Gets a collection of IMedia objects by ParentId
        /// </summary>
        /// <returns>IEnumerable of Media</returns>
        public IEnumerable<Media> GetAllAt(int id)
        {
            var mediaItems = _mediaService.GetChildren(id)
                .Where(m => m.ContentType.Equals(_contentTypeService.GetMediaType(PackageConstants.ImageAlias)));

            return mediaItems.Cast<Media>().ToList();
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
        /// Moves an IMedia object to a new location
        /// </summary>
        /// <param name="media">media to move</param>
        /// <param name="parentId">id of a new location</param>
        public void Move(IMedia media, int parentId)
        {
            _mediaService.Move(media, parentId);
        }

        /// <summary>
        /// Update Media
        /// </summary>
        /// <param name="id">Media Id</param>
        public void Update(int id, int actualSize)
        {
            // httpContext is null when optimization on upload
            // https://our.umbraco.org/projects/backoffice-extensions/tinifier/bugs/90472-error-systemargumentnullexception-value-cannot-be-null
            if (HttpContext.Current == null)
                HttpContext.Current = HttpContextHelper.CreateHttpContext
                    (new HttpRequest("", "http://localhost/", ""), new HttpResponse(new StringWriter()));

            if (_mediaService.GetById(id) is Media mediaItem)
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
            var historyIds = _database.Fetch<string>(query);

            var pardesIds = new List<int>();

            foreach (var historyId in historyIds)
            {
                if (int.TryParse(historyId, out var parsedId))
                {
                    pardesIds.Add(parsedId);
                }
            }

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                             Where(item => pardesIds.Contains(item.Id));

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
                foreach (var media in items)
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
            /*  var mediaItems = _mediaService.GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id);
              var numberOfItems = mediaItems.Count();

              return numberOfItems;*/
            var numberOfImages = Directory.EnumerateFiles(HttpContext.Current.Server.MapPath("/media/"), "*.*", SearchOption.AllDirectories)
                .Where(file => !file.ToLower().EndsWith("config")).Count();
            return numberOfImages;
        }

        /// <summary>
        /// Get Count of Optimized Images
        /// </summary>
        /// <returns>Number of optimized Images</returns>
        public int AmounthOfOptimizedItems()
        {
            var query = new Sql("SELECT ImageId FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var historyIds = _database.Fetch<string>(query);

            /*    var mediaItems = _mediaService.
                                 GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                                 Where(item => historyIds.Contains(item.Id));*/

            return historyIds.Count; //mediaItems.Count();
        }

        public IEnumerable<TImage> GetTopOptimizedImages()
        {
            var query = new Sql("SELECT ImageId, OriginSize, OptimizedSize, OccuredAt FROM TinifierResponseHistory WHERE IsOptimized = 'true'");
            var optimizedImages = _database.Fetch<TopImagesModel>(query);
            var historyIds = optimizedImages.OrderByDescending(x => (x.OriginSize - x.OptimizedSize)).Take(50).OrderByDescending(x => x.OccuredAt).Select(y => y.ImageId);

            var pardesIds = new List<int>();
            var croppedIds = new List<string>();

            foreach (var historyId in historyIds)
            {
                if (int.TryParse(historyId, out var parsedId))
                {
                    pardesIds.Add(parsedId);
                }
                else
                {
                    croppedIds.Add(historyId);
                }
            }

            var mediaItems = _mediaService.
                             GetMediaOfMediaType(_contentTypeService.GetMediaType(PackageConstants.ImageAlias).Id).
                             Where(item => pardesIds.Contains(item.Id));

            var images = new List<TImage>();

            foreach(var media in mediaItems)
            {
                var custMedia = new TImage
                {
                    Id = media.Id.ToString(),
                    Name = media.Name,
                    AbsoluteUrl = GetAbsoluteUrl(media as Media)
                };

                images.Add(custMedia);
            }

            foreach (var crop in croppedIds)
            {
                var custMedia = new TImage
                {
                    Id = crop,
                    Name = Path.GetFileName(crop),
                    AbsoluteUrl = crop
                };

                images.Add(custMedia);
            }

            return images;
        }

        protected string GetAbsoluteUrl(Media uMedia)
        {
            var umbHelper = new UmbracoHelper(UmbracoContext.Current);
            var content = umbHelper.Media(uMedia.Id);
            var imagerUrl = content.Url;
            return imagerUrl;
        }

        public class CustomMedia{

            public string Id { get; set; }

            public string Name { get; set; }
        }
    }
}
