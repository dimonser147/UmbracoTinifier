using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models;
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

            if (ex is EntityNotFoundException || ex is NotSupportedExtensionException ||
                ex is ConcurrentOptimizingException || ex is NotSuccessfullRequestException ||
                ex is HttpRequestException)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new TNotification("Tinifier Oops", ex.Message, Umbraco.Core.Events.EventMessageType.Error)
                    {
                        sticky = true,
                    });
            }
            else
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new TNotification("Tinifier unknown error", GetUnknownErrorMessage(ex), Umbraco.Core.Events.EventMessageType.Error)
                    {
                        sticky = true,
                        url = "https://our.umbraco.org/projects/backoffice-extensions/tinifier/bugs/"
                    });
            }

            LogHelper.Error(GetType(), ex.StackTrace, ex);
        }

        private string GetUnknownErrorMessage(Exception ex)
        {
            return $"{ex.Message}. We logged the error, if you are a hero, please take it and post in the forum (just click on this message)";
        }
    }
}
