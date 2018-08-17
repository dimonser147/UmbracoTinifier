using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Tinifier.Core.Infrastructure
{
    public static class HttpContextHelper
    {
        public static HttpContext CreateHttpContext(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }

        public static UmbracoContext CreateEnsureUmbracoContext()
        {
            var httpContextCurrent = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("dummy.aspx", "", new StringWriter())));
            return UmbracoContext.EnsureContext(
                httpContextCurrent,
                ApplicationContext.Current,
                new WebSecurity(httpContextCurrent, ApplicationContext.Current),
                UmbracoConfig.For.UmbracoSettings(),
                UrlProviderResolver.Current.Providers,
                false
            );
        }
    }
}
