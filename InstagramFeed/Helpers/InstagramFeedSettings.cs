using System;
using System.Collections.Generic;
using System.Configuration;

namespace InstagramFeed
{
    public class InstagramFeedSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string InstagramClientId {get;set;}
        
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> HashTags {get;set;}

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> AllowedOrigins { get; set; }

        /// <summary>
        /// The number of images per page to return when querying this api.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int InstagramPollInterval { get; set; }

        /// <summary>
        /// Default is true. If true, instagram's content is polled every this.InstagramPollInterval seconds, and is triggered
        /// when Images are requested from this API. If false, the PollInstagram method must be regularly called from some 
        /// external service. The latter is recommended, as it ensures Instagram content is polled at off peak times, and it 
        /// also removes the lag a user getting images will experience should she hit the API and trigger a poll.
        /// </summary>
        public bool PollInterally { get; set; }

        /// <summary>
        /// Use this if you want to authenticate calls to API. Request must have this cookie or header, presence is enough, value is ignored.
        /// </summary>
        public string AdminKey {get;set;}
        
        public bool SingleVotePerUser {get;set;}

        public DateTime? StartDate { get; set; }

        private static InstagramFeedSettings _settings;

        /// <summary>
        /// Gets instance of config section.
        /// </summary>
        /// <returns></returns>
        public static InstagramFeedSettings Instance
        {
            get
            {
                if (_settings == null)
                {
                    const string settingsNodeName = "instagramFeedSettings";
                    _settings = ConfigurationManager.GetSection(settingsNodeName) as InstagramFeedSettings;
                    if (_settings == null)
                        throw new ConfigurationErrorsException(string.Format("App.config is missing expected section '{0}'.", settingsNodeName));
                }

                return _settings;
             }
        }
    }
}
