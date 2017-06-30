using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Models.Service;
using Tinifier.Core.Repository.Repository;
using Tinifier.Core.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Tinifier.Core.Services.Services
{
    public class ImageService : IImageService
    {
        private readonly TImageRepository _imageRepository;
        private readonly IHistoryService _historyService;
        private readonly JavaScriptSerializer _serializer;

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _historyService = new HistoryService();
            _serializer = new JavaScriptSerializer();
        }

        public IEnumerable<TImage> GetAllImages()
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetAll();

            foreach (var item in imagesMedia)
            {
                var path = item.GetValue("umbracoFile").ToString();

                var image = new TImage
                {
                    Id = item.Id,
                    Name = item.Name,
                    Url = GetUrl(path)
                };

                images.Add(image);
            }

            return images;
        }

        public TImage GetImageById(int id)
        {
            var image = _imageRepository.GetByKey(id);

            if (image == null)
            {
                throw new EntityNotFoundException($"Image with such id doesn't exist. Id: {id}");
            }

            if(!CheckExtension(image.Name))
            {
                throw new Infrastructure.Exceptions.NotSupportedException(PackageConstants.NotSupported);
            }

            var path = image.GetValue("umbracoFile").ToString();

            var tImage = new TImage
            {
                Id = id,
                Name = image.Name,
                Url = GetUrl(path)
            };

            return tImage;
        }

        public void UpdateImage(TImage image, byte[] bytesArray)
        {
            var mediaService = ApplicationContext.Current.Services.MediaService;
            var mediaItem = mediaService.GetById(image.Id) as Media;

            System.IO.File.Delete(HttpContext.Current.Server.MapPath($"~{image.Url}"));
            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~{image.Url}"), bytesArray);

            if (mediaItem != null)
            {
                mediaItem.UpdateDate = DateTime.UtcNow;
                _imageRepository.UpdateItem(mediaService, mediaItem);
            }
        }

        public IEnumerable<TImage> GetAllOptimizedImages()
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetOptimizedItems().OrderByDescending(x => x.UpdateDate);

            foreach (var item in imagesMedia)
            {
                var path = item.GetValue("umbracoFile").ToString();

                var image = new TImage
                {
                    Id = item.Id,
                    Name = item.Name,
                    Url = GetUrl(path)
                };

                images.Add(image);
            }

            return images;
        }

        public IEnumerable<TImage> GetImagesFromFolder(int folderId)
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetItemsFromFolder(folderId);

            foreach (var item in imagesMedia)
            {
                var path = item.GetValue("umbracoFile").ToString();

                var image = new TImage
                {
                    Id = item.Id,
                    Name = item.Name,
                    Url = GetUrl(path)
                };

                images.Add(image);
            }

            return images;
        }

        public bool CheckFolder(int itemId)
        {
            var folder = _imageRepository.GetByKey(itemId);

            if (!string.Equals(folder.ContentType.Alias, "Folder", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public bool CheckExtension(string source)
        {
            var fileName = source.ToLower();

            if(!(fileName.Contains(".png") || fileName.Contains(".jpg") || fileName.Contains(".jpe") || fileName.Contains(".jpeg")))
            {
                return false;
            }

            return true;
        }

        private string GetUrl(string path)
        {
            string url;

            if (!path.Contains("src"))
            {
                url = path;
            }
            else
            {
                var urlModel = _serializer.Deserialize<UrlModel>(path);
                url = urlModel.Src;
            }
            
            return url;
        }
    }
}
