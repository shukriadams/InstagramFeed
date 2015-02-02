using System;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace InstagramFeed
{
        /// <summary>
    /// Used to flag Controller methods as having a minimal security role
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class OriginPolicyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            if (!InstagramFeedSettings.Instance.AllowedOrigins.Any())
                return;

            HttpContext context = HttpContext.Current;

            if (InstagramFeedSettings.Instance.AllowedOrigins.All(r => r != context.Request.Headers.Get("origin")))
                throw new Exception("Origin not allowed.");
        }
    }
}
