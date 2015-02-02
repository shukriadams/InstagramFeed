using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramFeed
{
    public class InstagramController : ApiController
    {
        #region FIELDS

        private readonly IInstagramImages _datalayer;
        private readonly IImageVotes _votes;

        #endregion

        #region CTOR

        /// <summary>
        /// IOC constructor. Be sure to register concrete types for these interfaces.
        /// </summary>
        /// <param name="datalayer"></param>
        /// <param name="votes"></param>
        public InstagramController(IInstagramImages datalayer, IImageVotes votes)
        {
            _datalayer = datalayer;
            _votes = votes;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Calls Instagram's API to get latest images. This method is intended to be driven by a schedule service, cronjob or some
        /// other reliable, regular process.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [OriginPolicy]
        public string PollInstagram() 
        {
            if (!AuthenticationHelper.IsAdmin())
                return "-1"; 

            return this.GetLatestFromInstagram().ToString();
        }

        /// <summary>
        /// Retrieves a block of images 
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="sort"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [OriginPolicy]
        public JToken GetImages(int page = 0, string sort = "", string search = "")
        {
            // update from instagram
            const string checkKey = "_lastCheck";
            DateTime? lastCheck = Cache.Get<DateTime?>(checkKey);
            if (InstagramFeedSettings.Instance.PollInterally 
                && (lastCheck == null || (DateTime.Now - lastCheck.Value).TotalSeconds > InstagramFeedSettings.Instance.InstagramPollInterval))
            {
                this.GetLatestFromInstagram();
                Cache.Add(checkKey, DateTime.Now);
            }

            if (!string.IsNullOrEmpty(search))
                search = search.Trim();

            // get all other images from parse backedn
            List<InstagramImage> results = new List<InstagramImage>();
            bool isAdmin = AuthenticationHelper.IsAdmin(); // if admin can be true

            IEnumerable<InstagramImage> images;
            if (string.IsNullOrEmpty(search))
                images = _datalayer.GetPage(isAdmin, null, sort, InstagramFeedSettings.Instance.PageSize, page);
            else
                images = _datalayer.GetPage(isAdmin, search, sort, InstagramFeedSettings.Instance.PageSize, 0);

            foreach (InstagramImage oldImage in images)
            {
                // get cached version, with metadata. this really needs to be reworked
                InstagramImage cachedImage = Get( oldImage.instagramImageId);
                results.Add(cachedImage);
            }

            return JToken.FromObject(new { tagCount = GetTagCount(), images = results, isAdmin });
        }

        /// <summary>
        /// Marks an image as blocked (moderated).
        /// </summary>
        /// <param name="id">Instragram image id.</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [OriginPolicy]
        public JToken BlockImage(string id) 
        {
            if (!AuthenticationHelper.IsAdmin())
                return JToken.FromObject(new { result = "1", message="permission error" }); 

            InstagramImage image = Get(id);
            if (image != null && !image.isBlocked)
            {
                image.isBlocked = true;
                _datalayer.Update(image);
                this.FlushImage(id);
            }
            return JToken.FromObject( new { result = "0" }); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [OriginPolicy]
        public JToken UnblockImage(string id)
        {
            if (!AuthenticationHelper.IsAdmin())
                return JToken.FromObject(new { result = "1", message = "permission error" }); 
            
            InstagramImage image = Get(id);
            if (image != null && image.isBlocked)
            {
                image.isBlocked = false;
                _datalayer.Update(image);
                this.FlushImage(id);
            }

            return JToken.FromObject( new { result = "0" } ); 
        }

        /// <summary>
        /// Votes an image up.
        /// </summary>
        /// <param name="id">Instramid of image to vote for.</param>
        /// <param name="person">Unique identifier of person voting. Optional.</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [OriginPolicy]
        public JToken Vote(string id, string person = "")
        {
            // check if user has voted
            if (InstagramFeedSettings.Instance.SingleVotePerUser)
            {
                if(_votes.GetVote(person, id) != null)
                    return JToken.FromObject(new { result = "1", message = "User has already voted." }); 
            }

            // create a distinct vote record
            ImageVote vote = new ImageVote
            {
                instagramImageId = id,
                voterId = person
            };
            _votes.Create(vote);

            // update the vote summary on the image itself
            InstagramImage image = Get(id);
            if (image != null)
            {
                image.votes = _votes.GetVoteCount(image.instagramImageId);
                _datalayer.Update(image);
            }

            // clear image from cache
            this.FlushImage(id);

            return JToken.FromObject(new { result = "0" });
        }

        #endregion

        #region METHODS PRIVATE

        /// <summary>
        /// Flushes an image from cache
        /// </summary>
        /// <param name="id"></param>
        private void FlushImage(string id)
        {
            string key = string.Format("image_{0}", id);
            Cache.Remove(key);
        }

        /// <summary>
        /// gets an image from cache or parse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private InstagramImage Get(string id)
        {
            // look in cache
            string key = string.Format("image_{0}", id);
            InstagramImage image = Cache.Get<InstagramImage>(key);
            if (image != null)
                return image;

            //QueryResult<InstagramImage> result = client.GetObjects<InstagramImage>(new { instagramImageId = id });
            //if (result == null || result.Results == null || !result.Results.Any())
            //    return null;

            InstagramImage i = _datalayer.GetByInstagramId(id);
            Cache.Add(key, i);
            return i;
        }

        /// <summary>
        /// Calls instagrams' API ; gets the latest images which implement the given hash tag.
        /// </summary>
        private int GetLatestFromInstagram()
        {
            string instagramId = InstagramFeedSettings.Instance.InstagramClientId;
            string hashTag = InstagramFeedSettings.Instance.HashTags.First();
            string url = string.Format("https://api.instagram.com/v1/tags/{0}/media/recent?client_id={1}", hashTag, instagramId);
            WebClient webClient = new WebClient();
            string rawJson = webClient.DownloadString(url);
            dynamic json = JsonConvert.DeserializeObject(rawJson);
            int imagesAdded = 0;
            JArray images = json["data"];

            foreach (var image in images)
            {
                if (image == null)
                    continue;

                // check if image exists from cache/parase
                InstagramImage persistedImaged = this.Get(image["id"].ToString());

                if (persistedImaged != null)
                    continue;

                string caption = Exists(image["caption"]) && Exists(image["caption"]["text"]) ? image["caption"]["text"].ToString() : string.Empty;
                string createdRaw = Exists(image["created_time"]) ? image["created_time"].ToString() : string.Empty;
                string imageLink = Exists(image["images"]) && Exists(image["images"]["standard_resolution"]) && Exists(image["images"]["standard_resolution"]["url"]) ? image["images"]["standard_resolution"]["url"].ToString() : string.Empty;
                string instagramImageId = Exists(image["id"]) ? image["id"].ToString() : string.Empty;
                string link = Exists(image["link"]) ? image["link"].ToString() : string.Empty;
                string thumbLink = Exists(image["images"]) && Exists(image["images"]["thumbnail"]) && Exists(image["images"]["thumbnail"]["url"]) ? image["images"]["thumbnail"]["url"].ToString() : string.Empty;
                string userFullName = Exists(image["user"]) && Exists(image["user"]["full_name"]) ? image["user"]["full_name"].ToString() : string.Empty;
                string userId = Exists(image["user"]) && Exists(image["user"]["id"]) ? image["user"]["id"].ToString() : string.Empty;
                string username = Exists(image["user"]) && Exists(image["user"]["username"]) ? image["user"]["username"].ToString() : string.Empty;
                string userThumbnail = Exists(image["user"]) && Exists(image["user"]["profile_picture"]) ? image["user"]["profile_picture"].ToString() : string.Empty;
                string latitude = Exists(image["location"]) && Exists(image["location"]["latitude"]) ? image["location"]["latitude"].ToString() : string.Empty;
                string longitude = Exists(image["location"]) && Exists(image["location"]["longitude"]) ? image["location"]["longitude"].ToString() : string.Empty;
                string createdTime = Exists(image["created_time"]) ? image["created_time"].ToString() : string.Empty;
                

                // enforce start date if set. Instagram uses Unix dates
                if (InstagramFeedSettings.Instance.StartDate.HasValue)
                {
                    int seconds;
                    int.TryParse(createdTime, out seconds);
                    System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                    if (seconds == 0)
                        continue; // can't parse unix date, something went wrong. todo : log this.
                    epoch = epoch.AddSeconds(seconds);
                    if (epoch < InstagramFeedSettings.Instance.StartDate.Value)
                        continue;
                }

                int created;
                Int32.TryParse(createdRaw, out created);

                persistedImaged = new InstagramImage
                {
                    caption = caption,
                    created = created,
                    imageLink = imageLink,
                    instagramImageId = instagramImageId,
                    link = link,
                    thumbLink = thumbLink,
                    userFullName = userFullName,
                    userThumbnail = userThumbnail,
                    userId = userId,
                    longitude = longitude,
                    latitude = latitude,
                    username = username
                };
                _datalayer.Create(persistedImaged);
                imagesAdded++;
            }

            return imagesAdded;
        }

        /// <summary>
        /// Gets the number of times the active tag has been used on instagram.
        /// </summary>
        /// <returns></returns>
        private string GetTagCount()
        {
            string hashTag = InstagramFeedSettings.Instance.HashTags.First(); ;
            string url = string.Format("https://api.instagram.com/v1/tags/{0}?client_id={1}", hashTag, InstagramFeedSettings.Instance.InstagramClientId);
            const string checkKey = "_tagCountCheck";
            const string hashTagCountKey = "hashTagCountKey";


            string count = Cache.Get<string>(hashTagCountKey);

            // update from instagram
            DateTime? lastCheck = Cache.Get<DateTime?>(checkKey);
            if (count == null || lastCheck == null || (DateTime.Now - lastCheck.Value).TotalSeconds > InstagramFeedSettings.Instance.InstagramPollInterval)
            {
                WebClient webClient = new WebClient();
                string rawJson = webClient.DownloadString(url);
                dynamic json = JsonConvert.DeserializeObject(rawJson);
                count = json["data"]["media_count"];

                Cache.Add(checkKey, DateTime.Now);
                Cache.Add(hashTagCountKey, count);
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool Exists(JToken token)
        {
            return token != null && token.ToString() != string.Empty;
        }

        #endregion
    }
}
