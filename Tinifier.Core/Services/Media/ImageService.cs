using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Services.BackendDevs;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.Settings;
using Tinifier.Core.Services.State;
using Tinifier.Core.Services.Statistic;
using Tinifier.Core.Services.TinyPNG;
using Tinifier.Core.Services.Validation;
using Tinifier.Core.Infrastructure;
using Umbraco.Core.IO;
using Tinifier.Core.Infrastructure.Exceptions;

namespace Tinifier.Core.Services.Media
{
    public class ImageService : BaseImageService, IImageService
    {
        private readonly IValidationService _validationService;
        private readonly TImageRepository _imageRepository;        
        private readonly IHistoryService _historyService;
        private readonly IStatisticService _statisticService;
        private readonly IStateService _stateService;
        private readonly ITinyPNGConnector _tinyPngConnectorService;
        private readonly IBackendDevsConnector _backendDevsConnectorService;
        private readonly ISettingsService _settingsService;

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _validationService = new ValidationService();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _backendDevsConnectorService = new BackendDevsConnectorService();
            _settingsService = new SettingsService();
        }

        public IEnumerable<TImage> GetAllImages()
        {
            return Convert(_imageRepository.GetAll());
        }

        public TImage GetImage(int id)
        {
            return GetImage(_imageRepository.Get(id));
        }

        public TImage GetImage(string path)
        {
            return GetImage(_imageRepository.Get(path));
        }

        public TImage GetCropImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new EntityNotFoundException();

            var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
            if (!PackageConstants.SupportedExtensions.Contains(fileExt))
                throw new NotSupportedExtensionException(fileExt);

            var tImage = new TImage
            {
                Id = path,
                Name = Path.GetFileName(path),
                AbsoluteUrl = path
            };

            return tImage;
        }

        private TImage GetImage(Umbraco.Core.Models.Media uMedia)
        {
            _validationService.ValidateExtension(uMedia);
            return base.Convert(uMedia);
        }

        public async Task OptimizeImageAsync(TImage image)
        {
            _stateService.CreateState(1);
            var tinyResponse = await _tinyPngConnectorService
                .TinifyAsync(image, base.FileSystem)
                .ConfigureAwait(false);
            if (tinyResponse.Output.Url == null)
            {
                _historyService.CreateResponseHistory(image.Id.ToString(), tinyResponse);
                return;
            }
            UpdateImageAfterSuccessfullRequest(tinyResponse, image, base.FileSystem);
            SendStatistic();
        }

        /// <summary>
        /// License
        /// Copyright © Backend Devs.
        /// During installation, you agree that we store a total number of images optimized for you.
        /// https://our.umbraco.org/projects/backoffice-extensions/tinifier/
        /// </summary>
        private void SendStatistic()
        {
            try
            {
                var userDomain = HttpContext.Current.Request.Url.Host;
                HostingEnvironment.QueueBackgroundWorkItem(stat => _backendDevsConnectorService.SendStatistic(userDomain));
            }
            catch (Exception)
            {
                return;
            }
        }

        public IEnumerable<TImage> GetOptimizedImages()
        {
            return Convert(_imageRepository.GetOptimizedItems().OrderByDescending(x => x.UpdateDate));
        }

        public IEnumerable<TImage> GetTopOptimizedImages()
        {
            // return Convert(_imageRepository.GetTopOptimizedImages().OrderByDescending(x => x.UpdateDate));

            
            var topOptimizedImages = _imageRepository.GetTopOptimizedImages();

            return topOptimizedImages;
        }

        public IEnumerable<TImage> GetFolderImages(int folderId)
        {
            return base.Convert(_imageRepository.GetItemsFromFolder(folderId));
        }

        public void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image, IFileSystem fs)
        {
            int.TryParse(image.Id, out var Id);

            // download optimized image
            var tImageBytes = TinyImageService.Instance.DownloadImage(tinyResponse.Output.Url);
            // preserve image metadata
            if (_settingsService.GetSettings().PreserveMetadata)
            {                
                byte[] originImageBytes = image.ToBytes(fs);
                PreserveImageMetadata(originImageBytes, ref tImageBytes);
            }
            // update physical file
            base.UpdateMedia(image, tImageBytes);
            // update history
            _historyService.CreateResponseHistory(image.Id.ToString(), tinyResponse);
            // update umbraco media attributes
            _imageRepository.Update(Id, tinyResponse.Output.Size);
            // update statistic
            var savedBytes = tinyResponse.Input.Size - tinyResponse.Output.Size;
            _statisticService.UpdateStatistic();
            // update tinifying state
            _stateService.UpdateState();
        }

        protected void PreserveImageMetadata(byte[] originImage, ref byte[] optimizedImage)
        {
            var originImg = (Image)new ImageConverter().ConvertFrom(originImage);
            var optimisedImg = (Image)new ImageConverter().ConvertFrom(optimizedImage);
            var srcPropertyItems = originImg.PropertyItems;
            foreach (var item in srcPropertyItems)
            {
                optimisedImg.SetPropertyItem(item);
            }

            using (var ms = new MemoryStream())
            {
                optimisedImg.Save(ms, optimisedImg.RawFormat);
                optimizedImage = ms.ToArray();
            }
        }
    }
}
