using System.Configuration;
using System.Xml;

namespace InstagramFeed.Parse
{
    public class ParseSettingsHandler : IConfigurationSectionHandler
    {
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            if (section == null || section.Attributes == null)
                throw new ConfigurationErrorsException("Required attributes missing.");

            // REQUIRED attributes
            if (section.Attributes["parseAppId"] == null || section.Attributes["parseAppId"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'parseAppId' missing from config.");
            string parseAppId = section.Attributes["parseAppId"].Value;

            if (section.Attributes["parseRestApiKey"] == null || section.Attributes["parseRestApiKey"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'parseRestApiKey' missing from config.");
            string parseRestApiKey = section.Attributes["parseRestApiKey"].Value;

            // optional
            bool forceDeleteDuplicates = false;
            if (section.Attributes["forceDeleteDuplicates"] != null && section.Attributes["forceDeleteDuplicates"].Value.Length > 0)
            {
                if (!bool.TryParse(section.Attributes["forceDeleteDuplicates"].Value, out forceDeleteDuplicates))
                    throw new ConfigurationErrorsException("'forceDeleteDuplicates' value is not a valid boolean.");
            }

            return new ParseSettings
            {
                ParseAppId = parseAppId,
                ParseRestApiKey = parseRestApiKey,
                ForceDeleteDuplicates = forceDeleteDuplicates
            };
        }
    }
}

