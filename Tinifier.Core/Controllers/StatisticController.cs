using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class StatisticController : UmbracoAuthorizedApiController
    {
        private readonly ISettingsService _settingsService;
        private readonly IStatisticService _statisticService;

        public StatisticController()
        {
            _settingsService = new SettingsService();
            _statisticService = new StatisticService();
        }

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
