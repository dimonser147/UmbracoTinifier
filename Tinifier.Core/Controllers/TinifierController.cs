using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Application;
using Tinifier.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    public class TinifierController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public HttpResponseMessage GetTImage(int timageId)
        {

            TImage timage = TestRepo.Get(timageId);

            if (timage == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, timage);

        }
    }
}
