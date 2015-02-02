using System.Collections.Generic;
using Parse.Api;
using Parse.Api.Models;

namespace InstagramFeed.Parse
{
    public class InstagramImages : IInstagramImages
    {
        private readonly ParseRestClient _client;

        public InstagramImages() 
        {
            _client = new ParseRestClient(ParseSettings.Instance.ParseAppId, ParseSettings.Instance.ParseRestApiKey);
        }

        public void Initialize()
        {
            // Do nothing
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Create(InstagramFeed.InstagramImage image) 
        {
            InstagramFeed.Parse.InstagramImage persistedImaged = new InstagramFeed.Parse.InstagramImage
            {
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes
            };
            _client.CreateObject(persistedImaged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Update(InstagramFeed.InstagramImage image)
        {
            InstagramFeed.Parse.InstagramImage persistedImaged = new InstagramFeed.Parse.InstagramImage
            {
                ObjectId = image.id,
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes
            };
            _client.Update(persistedImaged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InstagramFeed.InstagramImage GetByInstagramId (string id)
        {
            QueryResult<InstagramFeed.Parse.InstagramImage> result = _client.GetObjects<InstagramFeed.Parse.InstagramImage>(new { instagramImageId = id });
            if (result == null || result.Results == null || result.Results.Count == 0)
                return null;

            // delete duplicates if found. This is a self cleaning feature to catch when the same instagram image is accidentally imported more than once.
            if (ParseSettings.Instance.ForceDeleteDuplicates && result.Results.Count > 1 )
            {
                for (int i = 0 ; i < result.Results.Count ; i ++)
                {
                    if (i == 0)
                        continue;
                    var image = result.Results[i];
                    _client.DeleteObject(image);

                    // todo : log this
                }
            }

            return this.Load(result.Results[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private InstagramFeed.InstagramImage Load(InstagramFeed.Parse.InstagramImage image) 
        {
            return new InstagramFeed.InstagramImage
            {
                id = image.ObjectId,
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes
            };
        }

        public IEnumerable<InstagramFeed.InstagramImage> GetPage(bool isAdmin, string search, string sortBy, int pageSize, int pageIndex)
        {
            // get all other images from parse backedn
            QueryResult<InstagramFeed.Parse.InstagramImage> results;
            object query;
            sortBy = sortBy == "votes" ? "-votes" : "-created"; // - is Parse syntax for descending order
            if (string.IsNullOrEmpty(search))
            {
                if (isAdmin)
                    query = new { };
                else
                    query = new { isBlocked = false };

                results = _client.GetObjects<InstagramFeed.Parse.InstagramImage>(query, sortBy, pageSize, pageIndex * pageSize);
            }
            else
            {
                Constraint constraint = new Constraint { Regex = search, RegexOptions = "i" };
                if (isAdmin)
                    query = new { userFullName = constraint };
                else
                    query = new { isBlocked = false, userFullName = constraint };

                results = _client.GetObjects<InstagramFeed.Parse.InstagramImage>(query, sortBy, pageSize, pageIndex * pageSize);
            }

            List<InstagramFeed.InstagramImage> list = new List<InstagramFeed.InstagramImage>();
            if (results == null || results.Results == null)
                return list;

            foreach (var i in results.Results) 
                list.Add(this.Load(i));

            return list;
        }
    }
}
