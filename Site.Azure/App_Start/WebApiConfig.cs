﻿using System.Web.Http;

namespace Site.Azure
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }
            );

            // disable xml
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
