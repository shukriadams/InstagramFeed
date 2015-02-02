using System.Web.Http;
using InstagramFeed;
using InstagramFeed.Parse;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Site.Parse
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // IOC stuff
            var container = new Container();
            container.RegisterWebApiRequest<IInstagramImages, InstagramImages>();
            container.RegisterWebApiRequest<IImageVotes, ImageVotes>();

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // register api routes
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
