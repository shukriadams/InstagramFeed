using System.Configuration;

namespace InstagramFeed.Azure
{
    public class AzureSettings
    {
        public string ConnectionString { get; set; }

        private static AzureSettings _settings;

        /// <summary>
        /// Gets instance of config section.
        /// </summary>
        /// <returns></returns>
        public static AzureSettings Instance
        {
            get
            {
                if (_settings == null)
                {
                    const string settingsNodeName = "instagramFeedAzureSettings";
                    _settings = ConfigurationManager.GetSection(settingsNodeName) as AzureSettings;
                    if (_settings == null)
                        throw new ConfigurationErrorsException(string.Format("App.config is missing expected section '{0}'.", settingsNodeName));
                }

                return _settings;
            }
        }
    }
}
