using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Services.BackendDevs;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.State;
using Tinifier.Core.Services.Statistic;
using Tinifier.Core.Services.TinyPNG;
using Tinifier.Core.Services.Validation;
using Umbraco.Core.IO;

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

        public ImageService()
        {
            _imageRepository = new TImageRepository();
            _validationService = new ValidationService();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _backendDevsConnectorService = new BackendDevsConnectorService();
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

        private TImage GetImage(Umbraco.Core.Models.Media uMedia)
        {
            if (uMedia == null)
                throw new EntityNotFoundException(PackageConstants.ImageNotExists);

            if (!_validationService.ValidateExtension(uMedia.Name))
                throw new NotSupportedExtensionException(PackageConstants.NotSupported);

            return base.Convert(uMedia);
        }

        public async void OptimizeImage(TImage image)
        {
            var userDomain = HttpContext.Current.Request.Url.Host;
            _stateService.CreateState(1);
            var tinyResponse = await _tinyPngConnectorService.SendImageToTinyPngService(image, base.FileSystem);

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

        public IEnumerable<TImage> GetOptimizedImages()
        {
            return Convert(_imageRepository.GetOptimizedItems().OrderByDescending(x => x.UpdateDate));
        }

        public IEnumerable<TImage> GetFolderImages(int folderId)
        {
            return base.Convert(_imageRepository.GetItemsFromFolder(folderId));
        }

        public void UpdateImageAfterSuccessfullRequest(TinyResponse tinyResponse, TImage image)
        {
            // download optimized image
            var tImageBytes = TinyImageService.Instance.DownloadImage(tinyResponse.Output.Url);                        
            // update physical file
            base.UpdateMedia(image, tImageBytes);
            // update history
            _historyService.CreateResponseHistory(image.Id, tinyResponse);
            // update umbraco media attributes
            _imageRepository.Update(image.Id);
            // update statistic
            var savedBytes = tinyResponse.Input.Size - tinyResponse.Output.Size;
            _statisticService.UpdateStatistic(savedBytes);
            // update tinifying state
            _stateService.UpdateState();
        }
    }
}
