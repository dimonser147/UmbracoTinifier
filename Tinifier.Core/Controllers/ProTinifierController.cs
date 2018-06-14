using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tinifier.Core.Filters;
using Tinifier.Core.Models;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Media.Organizers;
using Umbraco.Core.Events;

namespace Tinifier.Core.Controllers
{
    [ExceptionFilter]
    public class ProTinifierController : TinifierController
    {
        private readonly IMediaHistoryService _mediaHistoryService;

        public ProTinifierController()
        {
            _mediaHistoryService = new ImageService();
        }

        [HttpGet]
        public HttpResponseMessage OrganizeImages(int folderId)
        {
            try
            {
                var organizer = new ByUploadedDateImageOrganizer(folderId);
                organizer.Organize();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return GetErrorNotification(ex.Message, HttpStatusCode.InternalServerError, EventMessageType.Error);
            }
        }

        [HttpGet]
        public HttpResponseMessage DiscardOrganizing()
        {
            try
            {
                _mediaHistoryService.DiscardOrganizing();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return GetErrorNotification(ex.Message, HttpStatusCode.InternalServerError, EventMessageType.Error);
            }
        }


        private HttpResponseMessage GetErrorNotification(string message, HttpStatusCode httpStatusCode, EventMessageType eventMessageType)
        {
            return Request.CreateResponse(httpStatusCode,
                    new TNotification("Tinifier Oops", message, eventMessageType) { sticky = true });
        }
    }
}
