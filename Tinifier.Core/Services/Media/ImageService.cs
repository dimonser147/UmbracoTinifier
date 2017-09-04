using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Models.Services;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Services.BackendDevs;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.State;
using Tinifier.Core.Services.Statistic;
using Tinifier.Core.Services.TinyPNG;
using Tinifier.Core.Services.Validation;

namespace Tinifier.Core.Services.Media
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
        private readonly IBackendDevsConnector _backendDevsConnectorService;

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _validationService = new ValidationService();
            _serializer = new JavaScriptSerializer();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _backendDevsConnectorService = new BackendDevsConnectorService();
        }

        public IEnumerable<TImage> GetAllImages()
        {
            var images = new List<TImage>();

            foreach (var item in _imageRepository.GetAll())
            {
                var path = item.GetValue(PackageConstants.UmbracoFileAlias).ToString();

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

        public TImage GetImage(int id)
        {
            var image = _imageRepository.Get(id);

            if (image == null)
                throw new EntityNotFoundException(PackageConstants.ImageNotExists + id);

            if (!_validationService.ValidateExtension(image.Name))
                throw new NotSupportedExtensionException(PackageConstants.NotSupported);

            var path = image.GetValue(PackageConstants.UmbracoFileAlias).ToString();

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
            var userDomain = HttpContext.Current.Request.Url.Host;
            _stateService.CreateState(1);
            var tinyResponse = await _tinyPngConnectorService.SendImageToTinyPngService(GetUrl(image.Url));

            if (tinyResponse.Output.Url == null)
            {
                _historyService.CreateResponseHistory(image.Id, tinyResponse);
                return;
            }

            UpdateImageAfterSuccessfullRequest(tinyResponse, image);

            try
            {
                HostingEnvironment.QueueBackgroundWorkItem(stat => _backendDevsConnectorService.SendStatistic(userDomain));
            }
            catch (NotSuccessfullRequestException)
            {
                return;
            }
        }

        public TImage GetImage(string path)
        {
            var image = _imageRepository.Get(path);

            if (image == null)
                throw new EntityNotFoundException(PackageConstants.ImageWithPathNotExists + path);

            if (!_validationService.ValidateExtension(image.Name))
                throw new NotSupportedExtensionException(PackageConstants.NotSupported);

            var umbracoFilepath = image.GetValue(PackageConstants.UmbracoFileAlias).ToString();

            var tImage = new TImage
            {
                Id = image.Id,
                Name = image.Name,
                Url = GetUrl(umbracoFilepath)
            };

            return tImage;
        }

        public IEnumerable<TImage> GetOptimizedImages()
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetOptimizedItems().OrderByDescending(x => x.UpdateDate);

            foreach (var item in imagesMedia)
            {
                var path = item.GetValue(PackageConstants.UmbracoFileAlias).ToString();

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

        public IEnumerable<TImage> GetFolderImages(int folderId)
        {
            var images = new List<TImage>();
            var imagesMedia = _imageRepository.GetItemsFromFolder(folderId);

            foreach (var item in imagesMedia)
            {
                var path = item.GetValue(PackageConstants.UmbracoFileAlias).ToString();

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

        public void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image)
        {
            var tinyImageBytes = TinyImageService.Instance.GetTinyImage(tinyResponse.Output.Url);
            var savedBytes = tinyResponse.Input.Size - tinyResponse.Output.Size;

            _historyService.CreateResponseHistory(image.Id, tinyResponse);
            UpdateImage(image, tinyImageBytes);
            _statisticService.UpdateStatistic(savedBytes);
            _stateService.UpdateState();
        }

        private void UpdateImage(TImage image, byte[] bytesArray)
        {
            var path = HttpContext.Current.Server.MapPath(string.Concat("~", image.Url));

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Dispose();
            }

            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllBytes(path, bytesArray);
            _imageRepository.Update(image.Id);
        }

        private string GetUrl(string path)
        {
            string url;

            if (!path.Contains(PackageConstants.Src))
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
