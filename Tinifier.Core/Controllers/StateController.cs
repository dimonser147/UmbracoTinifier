using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Services.State;
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

        /// <summary>
        /// Get current tinifing state
        /// </summary>
        /// <returns>Response(StatusCode, state)</returns>
        [HttpGet]
        public HttpResponseMessage GetCurrentTinifingState()
        {
            var state = _stateService.GetState();

            return Request.CreateResponse(HttpStatusCode.OK, state);
        }
    }
}
