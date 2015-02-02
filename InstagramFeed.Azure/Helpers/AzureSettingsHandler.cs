using System.Configuration;
using System.Xml;

namespace InstagramFeed.Azure
{
    public class AzureSettingsHandler : IConfigurationSectionHandler
    {
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            if (section == null || section.Attributes == null)
                throw new ConfigurationErrorsException("Required attributes missing.");

            // REQUIRED attributes
            if (section.Attributes["connectionString"] == null || section.Attributes["connectionString"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'connectionString' missing from config.");
            string connectionString = section.Attributes["connectionString"].Value;

            return new AzureSettings
            {
                ConnectionString = connectionString
            };
        }
    }
}
