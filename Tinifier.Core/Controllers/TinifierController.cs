using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class TinifierController : UmbracoAuthorizedApiController
    {
        private readonly IImageService _imageService;
        private readonly ITinyPNGConnector _tinyPngConnectorService;
        private readonly IHistoryService _historyService;
        private readonly ISettingsService _settingsService;
        private readonly IStatisticService _statisticService;
        private readonly IStateService _stateService;
        private readonly IValidationService _validationService;

        public TinifierController()
        {
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _settingsService = new SettingsService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
            _validationService = new ValidationService();
        }

        [HttpGet]
        public HttpResponseMessage GetTImage(int timageId)
        {
            TImage timage;

            try
            {
                timage = _imageService.GetImageById(timageId);
            }
            catch (NotSupportedException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            var history = _historyService.GetHistoryForImage(timageId);

            return Request.CreateResponse(HttpStatusCode.OK, new { timage, history });
        }

        [HttpGet]
        public async Task<HttpResponseMessage> TinyTImage(int itemId)
        {          
            HttpResponseMessage responseMessage;
            _settingsService.CheckIfSettingExists();
            var checkIfFolder = _validationService.CheckFolder(itemId);

            if (checkIfFolder)
            {
                _validationService.CheckConcurrentOptimizing();
                responseMessage = await TinifyImagesFromFolder(itemId);
            }
            else
            {               
                responseMessage = await TinifyOneImage(itemId);
            }

            return responseMessage;      
        }

        private async Task<HttpResponseMessage> TinifyImagesFromFolder(int itemId)
        {
            var images = _imageService.GetImagesFromFolder(itemId);
            var imagesList = _historyService.CheckImageHistory(images);

            if (imagesList.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AllImagesAlreadyOptimized);
            }

            _stateService.CreateState(itemId, imagesList.Count);
            var responseMessage = await CallTinyPngService(imagesList, SourceTypes.Folder);

            return responseMessage;
        }

        private async Task<HttpResponseMessage> TinifyOneImage(int itemId)
        {
            var imagesList = new List<TImage>();
            var image = _imageService.GetImageById(itemId);
            _validationService.CheckExtension(image.Name);
            var imageHistory = _historyService.GetHistoryForImage(image.Id);

            if (imageHistory != null)
            {
                if (imageHistory.IsOptimized)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AlreadyOptimized);
                }
            }

            imagesList.Add(image);
            var responseMessage = await CallTinyPngService(imagesList, SourceTypes.Image);

            return responseMessage;
        }

        private async Task<HttpResponseMessage> CallTinyPngService(IEnumerable<TImage> imagesList, SourceTypes sourceType)
        {
            foreach (var image in imagesList)
            {
                var tinyResponse = await _tinyPngConnectorService.SendImageToTinyPngService(image.Url);

                if (tinyResponse.Output.Url == null)
                {
                    _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);

                    if (sourceType == SourceTypes.Image)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.NotSuccessfullRequest);
                    }

                    _stateService.UpdateState();
                    continue;
                }

                _imageService.UpdateImageAfterSuccessfullRequest(tinyResponse, image, sourceType);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { PackageConstants.SuccessOptimized, sourceType});
        }
    }
}
