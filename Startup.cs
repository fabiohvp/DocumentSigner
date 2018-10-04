using DocumentSigner.Attributes;
using Owin;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DocumentSigner
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            var cors = new EnableCorsAttribute("*", "*", "*");

            config.EnableCors(cors);

            config.Filters
                .Add(new ExceptionResponseFilterAttribute());

            config.Routes
                .MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{action}/{parameters}",
                    defaults: new
                    {
                        parameters = RouteParameter.Optional
                    }
                );

            var xmlFormatter = config.Formatters
                .OfType<XmlMediaTypeFormatter>()
                .First();

            var jsonFormatter = config.Formatters
                .OfType<JsonMediaTypeFormatter>()
                .First();

            config.Formatters.Remove(xmlFormatter);
            //jsonFormatter.SerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
        }
    }
}