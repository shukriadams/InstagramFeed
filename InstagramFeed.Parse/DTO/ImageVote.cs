using Parse.Api.Models;

namespace InstagramFeed.Parse
{
    /// <summary>
    /// Vote record for an instagram image.
    /// </summary>
    public class ImageVote : ParseObject
    {
        /// <summary>
        /// Instagram id of image being voted for.
        /// </summary>
        public string instagramImageId { get; set; }

        /// <summary>
        /// Unique id of person voting. Optional.
        /// </summary>
        public string voterId { get; set; }
    }
}
