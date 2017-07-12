using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Services.Settings;
using Tinifier.Core.Services.Statistic;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class TinifierImagesStatisticController : UmbracoAuthorizedApiController
    {
        private readonly ISettingsService _settingsService;
        private readonly IStatisticService _statisticService;

        public TinifierImagesStatisticController()
        {
            _settingsService = new SettingsService();
            _statisticService = new StatisticService();
        }

        /// <summary>
        /// Get Images Statistic
        /// </summary>
        /// <returns>Response(StatusCode, {statistic, tsettings, requestLimit})</returns>
        [HttpGet]
        public HttpResponseMessage GetStatistic()
        {
            var statistic = _statisticService.GetStatistic();
            var tsetting = _settingsService.GetSettings();
            var monthlyRequestsLimit = PackageConstants.MonthlyRequestsLimit;

            return Request.CreateResponse(HttpStatusCode.OK, new { statistic, tsetting, monthlyRequestsLimit });
        }
    }
}
