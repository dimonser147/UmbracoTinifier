using System;
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
using Tinifier.Core.Services.BackendDevs;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Settings;
using Tinifier.Core.Services.State;
using Tinifier.Core.Services.TinyPNG;
using Tinifier.Core.Services.Validation;
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
        private readonly IStateService _stateService;
        private readonly IValidationService _validationService;
        private readonly IBackendDevsConnector _backendDevsConnectorService;

        public TinifierController()
        {
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _settingsService = new SettingsService();
            _stateService = new StateService();
            _validationService = new ValidationService();
            _backendDevsConnectorService = new BackendDevsConnectorService();
        }

        /// <summary>
        /// Get Image by id
        /// </summary>
        /// <param name="timageId">Image Id</param>
        /// <returns>Response(StatusCode, {image, history}}</returns>
        [HttpGet]
        public HttpResponseMessage GetTImage(int timageId)
        {
            TImage timage;

            try
            {
                timage = _imageService.GetImage(timageId);
            }
            catch (NotSupportedExtensionException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            var history = _historyService.GetImageHistory(timageId);
            return Request.CreateResponse(HttpStatusCode.OK, new {timage, history});
        }

        /// <summary>
        /// Tinify Image(s)
        /// </summary>
        /// <param name="imageRelativeUrls">Array of media items urls</param>
        /// <param name="mediaId">Media item id</param>
        /// <returns>Response(StatusCode, message)</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> TinyTImage([FromUri]string[] imageRelativeUrls, int mediaId)
        {
            HttpResponseMessage responseMessage;
            _settingsService.CheckIfSettingExists();
            // TODO: check any Tinifier activity
            _validationService.CheckConcurrentOptimizing();

            if (imageRelativeUrls.Length != 0)
            {
                responseMessage = await TinifyImages(imageRelativeUrls);
            }
            else
            {
                if (_validationService.IsFolder(mediaId))
                {
                    responseMessage = await TinifyFolder(mediaId);
                }
                else
                {
                    responseMessage = await TinifyImage(mediaId);
                }
            }

            return responseMessage;
        }

        /// <summary>
        /// Tinify folder By Id
        /// </summary>
        /// <param name="folderId">Folder Id</param>
        /// <returns>Response(StatusCode, message)</returns>
        private async Task<HttpResponseMessage> TinifyFolder(int folderId)
        {
            var images = _imageService.GetFolderImages(folderId);
            var imagesList = _historyService.CheckImageHistory(images);

            if (imagesList.Count == 0)           
                return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AllImagesAlreadyOptimized);     
                  
            _stateService.CreateState(imagesList.Count);
            return await CallTinyPngService(imagesList, SourceTypes.Folder);
        }

        /// <summary>
        /// Tinify Images by urls
        /// </summary>
        /// <param name="imagesRelativeUrls">Array of images urls</param>
        /// <returns>Response(StatusCode, message)</returns>
        private async Task<HttpResponseMessage> TinifyImages(IEnumerable<string> imagesRelativeUrls)
        {
            var nonOptimizedImages = new List<TImage>();

            foreach (var imageRelativeUrl in imagesRelativeUrls)
            {
                var image = _imageService.GetImage(imageRelativeUrl);
                var imageHistory = _historyService.GetImageHistory(image.Id);

                if (imageHistory != null && imageHistory.IsOptimized)
                    continue;

                nonOptimizedImages.Add(image);
            }

            if (nonOptimizedImages.Count == 0)           
                return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AllImagesAlreadyOptimized);

            _stateService.CreateState(nonOptimizedImages.Count);
            return await CallTinyPngService(nonOptimizedImages, SourceTypes.Folder);
        }

        /// <summary>
        /// Tinify image by Id
        /// </summary>
        /// <param name="imageId">Image Id</param>
        /// <returns>Response(StatusCode, message)</returns>
        private async Task<HttpResponseMessage> TinifyImage(int imageId)
        {            
            var imageById = _imageService.GetImage(imageId);
            _validationService.CheckExtension(imageById.Name);
            var notOptimizedImage = _historyService.GetImageHistory(imageById.Id);

            if (notOptimizedImage != null && notOptimizedImage.IsOptimized)
                return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AlreadyOptimized);

            var nonOptimizedImages = new List<TImage> {imageById};
            return await CallTinyPngService(nonOptimizedImages, SourceTypes.Image);
        }

        /// <summary>
        /// Create request to TinyPNG service and get response
        /// </summary>
        /// <param name="imagesList">Images that needs to be tinifing</param>
        /// <param name="sourceType">Folder or Image</param>
        /// <returns>Response(StatusCode, message)</returns>
        private async Task<HttpResponseMessage> CallTinyPngService(IEnumerable<TImage> imagesList, SourceTypes sourceType)
        {
            foreach (var image in imagesList)
            {
                var tinyResponse = await _tinyPngConnectorService.SendImageToTinyPngService(image.Url);

                if (tinyResponse.Output.Url == null)
                {
                    _historyService.CreateResponseHistoryItem(image.Id, tinyResponse);

                    if (sourceType == SourceTypes.Image)
                        return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.NotSuccessfullRequest);

                    _stateService.UpdateState();
                    continue;
                }

                _imageService.UpdateImageAfterSuccessfullRequest(tinyResponse, image, sourceType);
                _backendDevsConnectorService.SendStatistic(Request.RequestUri.GetLeftPart(UriPartial.Authority));
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { PackageConstants.SuccessOptimized, sourceType});
        }
    }
}
