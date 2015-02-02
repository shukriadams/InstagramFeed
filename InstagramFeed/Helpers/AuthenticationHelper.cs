using System.Web;

namespace InstagramFeed
{
    /// <summary>
    /// Quick and dirty admin permission checker.
    /// </summary>
    public class AuthenticationHelper
    {
        public static bool IsAdmin() 
        {
            // if user has a cookie or header declared as specified in the webconfig, user is assumed to be admin
            if (string.IsNullOrEmpty(InstagramFeedSettings.Instance.AdminKey))
            {
                // first try cookie
                if (HttpContext.Current.Request.Cookies[InstagramFeedSettings.Instance.AdminKey] != null)
                    return true;

                // then try header
                return HttpContext.Current.Request.Headers[InstagramFeedSettings.Instance.AdminKey] != null;
            }

            return false;
        }
    }
}
