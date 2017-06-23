using System;
using System.Collections.Generic;
using System.IO;
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
        private TImageRepository _imageRepository;
        private THistoryRepository _historyRepository;

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _historyRepository = new THistoryRepository();
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

        public TImage GetImageById(int Id)
        {
            var image = _imageRepository.GetByKey(Id);

            if (!string.IsNullOrEmpty(image.ContentType.Alias) && string.Equals(image.ContentType.Alias, "folder", StringComparison.OrdinalIgnoreCase))
            {
                throw new Infrastructure.Exceptions.NotSupportedException(PackageConstants.NotSupported);
            }

            CheckExtension(image.Name);
            var path = image.GetValue("umbracoFile").ToString();

            if (image == null)
            {
                throw new EntityNotFoundException($"Image with such id doesn't exist. Id: {Id}");
            }

            var tImage = new TImage
            {
                Id = Id,
                Name = image.Name,
                Url = GetUrl(path)
            };

            return tImage;
        }

        public void UpdateImage(TImage image, byte[] bytesArray)
        {
            var mediaService = ApplicationContext.Current.Services.MediaService;
            var mediaItem = mediaService.GetById(image.Id) as Media;

            using (var stream = new MemoryStream(bytesArray))
            {
                mediaService.SetMediaFileContent(image.Url, stream);
            }

            mediaItem.UpdateDate = DateTime.UtcNow;
            _imageRepository.UpdateItem(mediaService, mediaItem);
        }

        public IEnumerable<TImage> GetAllOptimizedImages()
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetOptimizedItems();

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

        public void CheckExtension(string source)
        {
            if(!(source.Contains(".png") || source.Contains(".jpg") || source.Contains(".jpe") || source.Contains(".jpeg")))
            {
                throw new Infrastructure.Exceptions.NotSupportedException(PackageConstants.NotSupported);
            }
        }

        private string GetUrl(string path)
        {
            string url;
            var serializer = new JavaScriptSerializer();

            if (!path.Contains("src"))
            {
                url = path;
            }
            else
            {
                var urlModel = serializer.Deserialize<UrlModel>(path);
                url = urlModel.Src;
            }
            
            return url;
        }
    }
}
