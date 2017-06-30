using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Infrastructure;
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

        public TinifierController()
        {
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _settingsService = new SettingsService();
            _statisticService = new StatisticService();
            _stateService = new StateService();
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
            _settingsService.CheckIfSettingExists();
            var imagesList = new List<TImage>();

            var image = _imageService.GetImageById(itemId);
            _imageService.CheckExtension(image.Name);
            var imageHistory = _historyService.GetHistoryForImage(image.Id);

            if (imageHistory != null)
            {
                if (imageHistory.IsOptimized)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AlreadyOptimized);
                }
            }

            imagesList.Add(image);
            var responseMessage = await CallTinyPngService(imagesList);

            return responseMessage;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> TinyTFolder(int folderId)
        {
            _settingsService.CheckIfSettingExists();
            _imageService.CheckFolder(folderId);
            var images = _imageService.GetImagesFromFolder(folderId);
            var imagesList = _historyService.CheckImageHistory(images);

            if (imagesList.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AllImagesAlreadyOptimized);
            }

            _stateService.CreateState(folderId);

            var responseMessage = await CallTinyPngService(imagesList);

            return responseMessage;
        }

        public HttpResponseMessage GetCurrentTinifingState()
        {
            var state = _stateService.GetState();
            return Request.CreateResponse(HttpStatusCode.OK, state);
        }

        private async Task<HttpResponseMessage> CallTinyPngService(IEnumerable<TImage> imagesList)
        {
            try
            {
                await GetTinyImage(imagesList);
            }
            catch (NotSuccessfullRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, PackageConstants.SuccessOptimized);
        }

        private async Task GetTinyImage(IEnumerable<TImage> imagesList)
        {
            foreach (var image in imagesList)
            {
                var imageBytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath($"~{image.Url}"));
                var tinyResponse = await _tinyPngConnectorService.TinifyByteArray(imageBytes);

                if (tinyResponse.Output.Url == null)
                {
                    _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);
                    throw new NotSuccessfullRequestException($"Request to TinyPNG with Image name {image.Name} was not successfull");
                }

                var tinyImageBytes = TinyImageService.Instance.GetTinyImage(tinyResponse.Output.Url);
                _imageService.UpdateImage(image, tinyImageBytes);
                _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);
                _statisticService.UpdateStatistic();
                _stateService.UpdateState();
            }            
        }
    }
}
