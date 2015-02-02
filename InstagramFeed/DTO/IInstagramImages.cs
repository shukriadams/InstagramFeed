using System.Collections.Generic;

namespace InstagramFeed
{
    public interface IInstagramImages
    {
        void Initialize();

        /// <summary>
        /// 
        /// </summary>
        void Create(InstagramImage image);

        /// <summary>
        /// 
        /// </summary>
        void Update(InstagramImage image);

        /// <summary>
        /// 
        /// </summary>
        InstagramImage GetByInstagramId(string id);

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<InstagramImage> GetPage(bool isAdmin, string search, string sortBy, int pageSize, int pageIndex);
    }
}
