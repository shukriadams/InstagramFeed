using System;
using Parse.Api.Models;

namespace InstagramFeed.Parse
{
    public class InstagramImage : ParseObject
    {
        public string id { get; set; }
        public string link { get; set; }
        public int created { get; set; }
        public string imageLink { get; set; }
        public string thumbLink { get; set; }
        public string caption { get; set; }
        public string instagramImageId { get; set; }
        public string userFullName { get; set; }
        public string userThumbnail { get; set; }
        public string username { get; set; }
        public string userId { get; set; }
        public bool isBlocked { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }


        /// <summary>
        /// summary of imagevote records, calculated on the fly
        /// </summary>
        public int votes { get; set; }
    }
}
