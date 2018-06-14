using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Services.BackendDevs;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.Media.Organizers;
using Tinifier.Core.Services.State;
using Tinifier.Core.Services.Statistic;
using Tinifier.Core.Services.TinyPNG;
using Tinifier.Core.Services.Validation;
using uMedia = Umbraco.Core.Models.Media;

namespace Tinifier.Core.Services.Media
{
    public class ImageService : BaseImageService, IImageService, IMediaHistoryService
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

        public IEnumerable<uMedia> GetAllImagesAt(int folderId)
        {
            return _imageRepository.GetAllAt(folderId);
        }

        public TImage GetImage(int id)
        {
            return GetImage(_imageRepository.Get(id));
        }

        public TImage GetImage(string path)
        {
            return GetImage(_imageRepository.Get(path));
        }

        private TImage GetImage(uMedia uMedia)
        {
            _validationService.ValidateExtension(uMedia);
            return Convert(uMedia);
        }

        public void Move(uMedia image, int parentId)
        {
            _imageRepository.Move(image, parentId);
        }

        public async Task OptimizeImageAsync(TImage image)
        {            
            _stateService.CreateState(1);
            var tinyResponse = await _tinyPngConnectorService.TinifyAsync(image, base.FileSystem).ConfigureAwait(false);
            if (tinyResponse.Output.Url == null)
            {
                _historyService.CreateResponseHistory(image.Id, tinyResponse);
                return;
            }
            UpdateImageAfterSuccessfullRequest(tinyResponse, image);
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
            return Convert(_imageRepository.GetTopOptimizedImages().OrderByDescending(x => x.UpdateDate));
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
            _imageRepository.Update(image.Id, tinyResponse.Output.Size);
            // update statistic
            var savedBytes = tinyResponse.Input.Size - tinyResponse.Output.Size;
            _statisticService.UpdateStatistic();
            // update tinifying state
            _stateService.UpdateState();
        }

        public void BackupMediaPaths(IEnumerable<uMedia> media)
        {
            var mediaHistoryRepo = new Repository.History.TMediaHistoryRepository();
            mediaHistoryRepo.DeleteAll();
            foreach (var m in media)
            {
                var mediaHistory = new TinifierMediaHistory
                {
                    MediaId = m.Id,
                    FormerPath = m.Path
                };
                mediaHistoryRepo.Create(mediaHistory);
            }
        }

        public void DiscardOrganizing()
        {
            var mediaHistoryRepo = new Repository.History.TMediaHistoryRepository();
            var media = mediaHistoryRepo.GetAll();
            foreach (var m in media)
            {
                // the path is stored as a string with comma separated IDs of media
                // where the last value is ID of current media, penultimate value is ID of its root, etc.
                // the first value is ID of the very root media
                var path = m.FormerPath.Split(',');
                var formerParentId = Int32.Parse(path[path.Length - 2]);
                var image = _imageRepository.Get(m.MediaId);
                Move(image, formerParentId);
            }
        }
    }
}
