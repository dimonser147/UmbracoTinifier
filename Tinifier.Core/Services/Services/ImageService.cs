using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Models.Service;
using Tinifier.Core.Repository.Repository;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.Core.Services.Services
{
    public class ImageService : IImageService
    {
        private readonly IValidationService _validationService;
        private readonly TImageRepository _imageRepository;
        private readonly JavaScriptSerializer _serializer;
        private readonly IHistoryService _historyService;
        private readonly IStatisticService _statisticService;
        private readonly IStateService _stateService;
        private readonly ITinyPNGConnector _tinyPngConnectorService;

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _validationService = new ValidationService();
            _serializer = new JavaScriptSerializer();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
        }

        public TImage GetImageById(int id)
        {
            var image = _imageRepository.GetByKey(id);

            if (image == null)
            {
                throw new EntityNotFoundException($"Image with such id doesn't exist. Id: {id}");
            }

            if(!_validationService.CheckExtension(image.Name))
            {
                throw new NotSupportedException(PackageConstants.NotSupported);
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

        public async void OptimizeImage(TImage image)
        {
            var tinyResponse = await _tinyPngConnectorService.SendImageToTinyPngService(GetUrl(image.Url));

            if (tinyResponse.Output.Url == null)
            {
                _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);

                return;
            }

            UpdateImageAfterSuccessfullRequest(tinyResponse, image, SourceTypes.Image);
        }

        public TImage GetImageByPath(string path)
        {
            var image = _imageRepository.GetByPath(path);

            if (image == null)
            {
                throw new EntityNotFoundException($"Image with such name doesn't exist. Name: {path}");
            }

            if (!_validationService.CheckExtension(image.Name))
            {
                throw new NotSupportedException(PackageConstants.NotSupported);
            }

            var umbracoFilepath = image.GetValue("umbracoFile").ToString();

            var tImage = new TImage
            {
                Id = image.Id,
                Name = image.Name,
                Url = GetUrl(umbracoFilepath)
            };

            return tImage;
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

        public void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, SourceTypes sourceType)
        {
            var tinyImageBytes = TinyImageService.Instance.GetTinyImage(tinyResponse.Output.Url);

            _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);
            UpdateImage(image, tinyImageBytes);
            _statisticService.UpdateStatistic();

            if (sourceType == SourceTypes.Folder)
            {
                _stateService.UpdateState();
            }
        }

        private void UpdateImage(TImage image, byte[] bytesArray)
        {
            System.IO.File.Delete(HttpContext.Current.Server.MapPath($"~{image.Url}"));
            System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~{image.Url}"), bytesArray);

            _imageRepository.UpdateItem(image.Id);
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
