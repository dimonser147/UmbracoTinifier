using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models;
using Tinifier.Core.Models.Service;
using Tinifier.Core.Repository;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Tinifier.Core.Services
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

            mediaItem.UpdateDate = DateTime.Now;
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
            if(!(source.Contains(".png") || source.Contains(".jpg")))
            {
                throw new Infrastructure.Exceptions.NotSupportedException("This extension not supported. Only png and jpg");
            }
        }

        private string GetUrl(string path)
        {
            var serializer = new JavaScriptSerializer();
            var urlModel = serializer.Deserialize<UrlModel>(path);
            var url = urlModel.Src;

            return url;
        }
    }
}
