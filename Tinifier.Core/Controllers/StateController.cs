using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class StateController : UmbracoAuthorizedApiController
    {
        private readonly IStateService _stateService;

        public StateController()
        {
            _stateService = new StateService();
        }

        [HttpGet]
        public HttpResponseMessage GetCurrentTinifingState()
        {
            var state = _stateService.GetState();
            return Request.CreateResponse(HttpStatusCode.OK, state);
        }
    }
}
