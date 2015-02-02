using Microsoft.WindowsAzure.Storage.Table;

namespace InstagramFeed.Azure
{
    /// <summary>
    /// Vote record for an instagram image.
    /// </summary>
    public class ImageVote : TableEntity
    {
        public ImageVote() 
        {

        }

        public ImageVote(string instagramImageId, string voterId)
        {
            this.PartitionKey = instagramImageId;
            this.RowKey = voterId;
        }


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

