using System.Configuration;

namespace InstagramFeed.Parse
{
    public class ParseSettings
    {
        public string ParseAppId { get; set; }

        public string ParseRestApiKey { get; set; }

        /// <summary>
        /// Try to delete potential duplicate instagram imports when found. Default is false.
        /// </summary>
        public bool ForceDeleteDuplicates { get; set; }

        private static ParseSettings _settings;

        /// <summary>
        /// Gets instance of config section.
        /// </summary>
        /// <returns></returns>
        public static ParseSettings Instance
        {
            get
            {
                if (_settings == null)
                {
                    const string settingsNodeName = "instagramFeedParseSettings";
                    _settings = ConfigurationManager.GetSection(settingsNodeName) as ParseSettings;
                    if (_settings == null)
                        throw new ConfigurationErrorsException(string.Format("App.config is missing expected section '{0}'.", settingsNodeName));
                }

                return _settings;
            }
        }
    }
}
