using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Tinifier.Core.Infrastructure.Exceptions;
using Umbraco.Core.Logging;

namespace Tinifier.Core.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;

            if (context.Exception is EntityNotFoundException)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            if(context.Exception is NotSupportedException)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, ex.Message);
            }

            if(context.Exception is ConcurrentOptimizingException)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            LogHelper.Error(GetType(), ex.StackTrace, ex);
        }
    }
}
