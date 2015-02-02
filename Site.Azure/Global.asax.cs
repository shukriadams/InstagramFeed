using System.Web.Http;
using InstagramFeed;
using InstagramFeed.Azure;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Site.Azure
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

            // initialize tables
            new InstagramImages().Initialize();
            new ImageVotes().Initialize();

            // register api routes
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
