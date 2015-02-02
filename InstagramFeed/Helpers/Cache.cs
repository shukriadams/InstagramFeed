using System.Web;

namespace InstagramFeed
{
    public class Cache
    {
        public static object Get(string key)
        {
            return HttpContext.Current.Cache[key];
        }
        public static T Get<T>(string key)
        {
            return (T)HttpContext.Current.Cache[key];
        }

        public static void Add(string key, object item) 
        {
            HttpContext.Current.Cache[key] = item;
        }

        public static void Add<T>(string key, T item)
        {
            if (item != null)
                HttpContext.Current.Cache[key] = item;
        }

        public static void Remove(string key)
        {
            if (HttpContext.Current.Cache[key] != null)
                HttpContext.Current.Cache.Remove(key);
        }
    }
}
