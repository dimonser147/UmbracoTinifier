using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models;
using Tinifier.Core.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class TinifierController : UmbracoAuthorizedApiController
    {
        private IImageService _imageService;
        private ITinyPNGConnector _tinyPngConnectorService;
        private IHistoryService _historyService;
        private ISettingsService _settingsService;

        public TinifierController()
        {
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _settingsService = new SettingsService();
        }

        [HttpGet]
        public HttpResponseMessage GetTImage(int timageId)
        {
            var timage = _imageService.GetImageById(timageId);
            var history = _historyService.GetHistoryForImage(timageId);

            return Request.CreateResponse(HttpStatusCode.OK, new { timage, history });
        }

        [HttpGet]
        public HttpResponseMessage GetTSetting()
        {
            var tsetting = _settingsService.GetSettings(); 

            if(tsetting == null)
            {
                tsetting = new TSetting();
            }

            return Request.CreateResponse(HttpStatusCode.OK, tsetting);
        }

        [HttpPost]
        public HttpResponseMessage CreateSettings(TSetting setting)
        {
            if(ModelState.IsValid)
            {
                _settingsService.CreateSettings(setting);
                return Request.CreateResponse(HttpStatusCode.Created);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        }

        [HttpGet]
        public HttpResponseMessage GetProperties(int timageId)
        {
            var history = _historyService.GetHistoryForImage(timageId);
            return Request.CreateResponse(HttpStatusCode.BadRequest, history);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> TinyTImage(int timageId)
        {
            var image = _imageService.GetImageById(timageId);
            _imageService.CheckExtension(image.Name);
            _settingsService.CheckIfSettingExists();
            var imageHistory = _historyService.GetHistoryForImage(image.Id);

            if(imageHistory != null)
            {
                if(imageHistory.IsOptimized)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Picture was optimized before");
                }                
            }

            var imageBytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath($"~{image.Url}"));            
            var tinyResponse = await _tinyPngConnectorService.TinifyByteArray(imageBytes);

            if(tinyResponse.Output.Url == null)
            {
                _historyService.CreateResponseHistoryItem(timageId, tinyResponse);
                return Request.CreateResponse(HttpStatusCode.BadRequest, tinyResponse.Output.Error);
            }

            var tinyImageBytes = TinyImageService.Instance.GetTinyImage(tinyResponse.Output.Url);
            _imageService.UpdateImage(image, tinyImageBytes);
            _historyService.CreateResponseHistoryItem(timageId, tinyResponse);

            return Request.CreateResponse(HttpStatusCode.OK, "Picture optimized succesfully");
        }
    }
}
