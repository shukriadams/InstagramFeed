using System;

namespace InstagramFeed
{
    [Serializable]
    public class InstagramImage 
    {
        // internal database id for record. This is NOT an instagram value
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// Unix datetime. 
        /// </summary>
        public int created { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string imageLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string thumbLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string caption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string instagramImageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userFullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userThumbnail { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isBlocked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// summary of imagevote records, calculated on the fly
        /// </summary>
        public int votes { get; set; }
    }
}
