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

        public TinifierController()
        {
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _tinyPngConnectorService = new TinyPNGConnectorService();
            _settingsService = new SettingsService();
            _statisticService = new StatisticService();
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

            return Request.CreateResponse(HttpStatusCode.OK, new {timage, history});
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
                return Request.CreateResponse(HttpStatusCode.Created, PackageConstants.ApiKeyMessage);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.ApiKeyError);
        }

        [HttpGet]
        public HttpResponseMessage GetStatistic()
        {
            var statistic = _statisticService.GetStatistic();
            var tsetting = _settingsService.GetSettings();
            var monthlyRequestsLimit = PackageConstants.MonthlyRequestsLimit;

            return Request.CreateResponse(HttpStatusCode.OK, new { statistic, tsetting, monthlyRequestsLimit });
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.AlreadyOptimized);
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
            _statisticService.UpdateStatistic();

            return Request.CreateResponse(HttpStatusCode.OK, PackageConstants.SuccessOptimized);
        }
    }
}
