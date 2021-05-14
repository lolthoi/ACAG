using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace ACAG.Abacus.CalendarConnector.Abacus
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Services.Replace(typeof(IExceptionLogger), new CustomExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());

            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new CustomAuthenticationHandler());
        }
    }
}
