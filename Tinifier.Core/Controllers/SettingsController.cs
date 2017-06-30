using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class SettingsController : UmbracoAuthorizedApiController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController()
        {
            _settingsService = new SettingsService();
        }

        [HttpGet]
        public HttpResponseMessage GetTSetting()
        {
            var tsetting = _settingsService.GetSettings();

            if (tsetting == null)
            {
                tsetting = new TSetting();
            }

            return Request.CreateResponse(HttpStatusCode.OK, tsetting);
        }

        [HttpPost]
        public HttpResponseMessage CreateSettings(TSetting setting)
        {
            if (ModelState.IsValid)
            {
                _settingsService.CreateSettings(setting);
                return Request.CreateResponse(HttpStatusCode.Created, PackageConstants.ApiKeyMessage);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, PackageConstants.ApiKeyError);
        }
    }
}
