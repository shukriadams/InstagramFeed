using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace InstagramFeed.Azure
{
    public class ImageVotes : IImageVotes
    {
        public void Initialize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ImageVote");
            table.CreateIfNotExists();
        }

        public void Create(InstagramFeed.ImageVote image) 
        {
            InstagramFeed.Azure.ImageVote persistedVote = new InstagramFeed.Azure.ImageVote
            {
                instagramImageId = image.instagramImageId,
                RowKey = Guid.NewGuid().ToString(),
                voterId = image.voterId
            };

            // Create the CloudTable object that represents the "people" table.
            // Create the table client.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ImageVote");
            TableOperation insertOperation = TableOperation.Insert(persistedVote);
            table.Execute(insertOperation);
        }

        public InstagramFeed.ImageVote GetVote(string voter, string instagramImageId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("ImageVote");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<InstagramFeed.Azure.ImageVote> rangeQuery = new TableQuery<InstagramFeed.Azure.ImageVote>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, instagramImageId),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, voter)));

            IEnumerable<InstagramFeed.Azure.ImageVote> result = table.ExecuteQuery(rangeQuery).ToList();
            if (!result.Any())
                return null;
            return this.Load(result.First());
        }

        public int GetVoteCount(string instagramImageId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("ImageVote");
            TableQuery<InstagramFeed.Azure.ImageVote> rangeQuery = new TableQuery<InstagramFeed.Azure.ImageVote>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, instagramImageId));

            return table.ExecuteQuery(rangeQuery).Count();
        }

        private InstagramFeed.ImageVote Load(InstagramFeed.Azure.ImageVote vote) 
        {
            return new InstagramFeed.ImageVote
            {
                instagramImageId = vote.instagramImageId,
                voterId = vote.voterId
            };
        }
    }
}
