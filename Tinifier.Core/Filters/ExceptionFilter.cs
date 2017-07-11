using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Infrastructure.Exceptions;
using Umbraco.Core.Logging;

namespace Tinifier.Core.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Custom exception filter
        /// </summary>
        /// <param name="context">HttpActionExecutedContext</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;

            if (context.Exception is EntityNotFoundException)
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, 
                    new { Message = ex.Message, Error = ErrorTypes.Error });

            if (context.Exception is NotSupportedExtensionException)
                context.Response = context.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, 
                    new { Message = ex.Message, Error = ErrorTypes.Error });

            if (context.Exception is ConcurrentOptimizingException)
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, 
                    new { Message = ex.Message, Error = ErrorTypes.Error });

            if(context.Exception is NotSuccessfullRequestException)
                context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new { Message = ex.Message, Error = ErrorTypes.Error });

            LogHelper.Error(GetType(), ex.StackTrace, ex);
        }
    }
}
