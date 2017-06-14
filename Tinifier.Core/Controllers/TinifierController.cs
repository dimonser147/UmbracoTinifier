using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models;
using Tinifier.Core.Models.API;
using Tinifier.Core.Services;
using Umbraco.Web.WebApi;

namespace Tinifier.Core.Controllers
{
    public class TinifierController : UmbracoAuthorizedApiController
    {
        private TImageRepository _repo;
        private JavaScriptSerializer _serializer;
        private ITCreateRequest _requestService;

        public TinifierController()
        {
            _serializer = new JavaScriptSerializer();
            _repo = new TImageRepository();
            _requestService = new TCreateRequestService();
        }

        [HttpGet]
        public HttpResponseMessage GetTImage(int timageId)
        {
            TImage timage;

            try
            {
                timage = _repo.GetItemById(timageId);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.StackTrace);
            }
           
            if (timage == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
                
            return Request.CreateResponse(HttpStatusCode.OK, timage);
        }

        [HttpGet]
        public HttpResponseMessage GetTSetting()
        {
            TSetting tsetting = new TSetting
            {
                ApiKey = "OrPba3PL6Q5tIAjoTxQZx1jnyf-qAXMw",
                EnableOptimizationOnUpload = true
            };

            return Request.CreateResponse(HttpStatusCode.OK, tsetting);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> TinyTImage(int timageId)
        {
            TinyResponse tinyResponse;
            var image = _repo.GetItemById(timageId);

            if (image == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var imageBytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath($"~{image.Url}"));
            
            try
            {
                var response = await _requestService.CreateRequestByteArray(imageBytes);
                tinyResponse = _serializer.Deserialize<TinyResponse>(response);

                var webClient = new WebClient();
                var tinyImageBytes = webClient.DownloadData(tinyResponse.Output.Url);
                _repo.UpdateItem(timageId, tinyImageBytes);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.StackTrace);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tinyResponse);
        }
    }
}
