using System;
using System.Configuration;
using System.Xml;

namespace InstagramFeed
{
    public class InstagramFeedSettingsHandler : IConfigurationSectionHandler
    {
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            if (section == null || section.Attributes == null)
                throw new ConfigurationErrorsException("Required attributes missing.");

            // REQUIRED attributes
            if (section.Attributes["instagramClientId"] == null || section.Attributes["instagramClientId"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'instagramClientId' missing from config.");
            string instagramClientId = section.Attributes["instagramClientId"].Value;

            if (section.Attributes["hashTags"] == null || section.Attributes["hashTags"].Value.Length == 0)
                throw new ConfigurationErrorsException("Required attribute 'hashTags' missing from config.");
            string[] hashTags = section.Attributes["hashTags"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // option attributes
            int pageSize = 12; // this is the default page size

            if (section.Attributes["pageSize"] != null && section.Attributes["pageSize"].Value.Length > 0) 
            {
                if (!Int32.TryParse(section.Attributes["pageSize"].Value, out pageSize))
                    throw new ConfigurationErrorsException("'pageSize' value is not a valid integer.");
            }

            string[] allowedOrigins = { };
            if (section.Attributes["allowedOrigins"] != null && section.Attributes["allowedOrigins"].Value.Length > 0)
            {
                allowedOrigins = section.Attributes["allowedOrigins"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            DateTime? startDate = null;
            if (section.Attributes["startDate"] != null && section.Attributes["startDate"].Value.Length > 0)
            {
                string rawDate = section.Attributes["startDate"].Value;
                if (rawDate.Length != 8)
                    throw new Exception("startDate must be a valid ISODate.");

                string rawYear = rawDate.Substring(0, 4);
                string rawMonth = rawDate.Substring(4, 2);
                string rawDay = rawDate.Substring(6, 2);

                int year;
                int month;
                int day;
                int.TryParse(rawYear, out year);
                int.TryParse(rawMonth, out month);
                int.TryParse(rawDay, out day);

                try
                {
                    startDate = new DateTime(year, month, day);
                }
                catch (FormatException ex)
                {
                    throw new ConfigurationErrorsException(string.Format("'{0}-{1}-{2}' did not produce a valid date' (ISO data YYYYMMDDD expected).", rawYear, rawMonth, rawDay), ex);
                }
            }

            int instagramPollInterval = 60; // value is in seconds
            if (section.Attributes["instagramPollInterval"] != null && section.Attributes["instagramPollInterval"].Value.Length > 0)
            {
                if (!Int32.TryParse(section.Attributes["instagramPollInterval"].Value, out instagramPollInterval))
                    throw new ConfigurationErrorsException("'instagramPollInterval' value is not a valid integer.");
            }

            string adminKey = section.Attributes["adminKey"] == null ? string.Empty : section.Attributes["adminKey"].Value;

            bool singleVotePerUser = false;
            if (section.Attributes["singleVotePerUser"] != null && section.Attributes["singleVotePerUser"].Value.Length > 0)
            {
                if (!bool.TryParse(section.Attributes["singleVotePerUser"].Value, out singleVotePerUser))
                    throw new ConfigurationErrorsException("'singleVotePerUser' value is not a valid boolean.");
            }

            bool pollInterally = true;
            if (section.Attributes["pollInterally"] != null && section.Attributes["pollInterally"].Value.Length > 0)
            {
                if (!bool.TryParse(section.Attributes["pollInterally"].Value, out pollInterally))
                    throw new ConfigurationErrorsException("'pollInterally' value is not a valid boolean.");
            }

            
            return new InstagramFeedSettings
            {
                InstagramClientId = instagramClientId,
                HashTags = hashTags,
                AdminKey = adminKey,
                PageSize = pageSize,
                SingleVotePerUser = singleVotePerUser,
                AllowedOrigins = allowedOrigins,
                StartDate = startDate,
                PollInterally = pollInterally,
                InstagramPollInterval = instagramPollInterval
            };
        }
    }
}
