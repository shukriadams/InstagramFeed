using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace InstagramFeed.Azure
{
    public class InstagramImages : IInstagramImages
    {
        public void Initialize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureSettings.Instance.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("InstagramImage");
            table.CreateIfNotExists();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Create(InstagramFeed.InstagramImage image)
        {
            InstagramFeed.Azure.InstagramImage persistedImaged = new InstagramFeed.Azure.InstagramImage
            {
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes,
                RowKey = image.instagramImageId,
            };
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureSettings.Instance.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("InstagramImage");
            TableOperation insertOperation = TableOperation.Insert(persistedImaged);
            table.Execute(insertOperation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void Update(InstagramFeed.InstagramImage image)
        {
            InstagramFeed.Azure.InstagramImage persistedImaged = new InstagramFeed.Azure.InstagramImage
            {
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes,
                RowKey = image.instagramImageId,
            };
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureSettings.Instance.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("InstagramImage");
            TableOperation replaceOperation = TableOperation.Replace(persistedImaged);
            table.Execute(replaceOperation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InstagramFeed.InstagramImage GetByInstagramId(string id)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureSettings.Instance.ConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("InstagramImage");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<InstagramFeed.Azure.InstagramImage> rangeQuery = new TableQuery<InstagramFeed.Azure.InstagramImage>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));

            IEnumerable<InstagramFeed.Azure.InstagramImage> result = table.ExecuteQuery(rangeQuery).ToList();
            if (!result.Any())
                return null;
            return this.Load(result.First());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private InstagramFeed.InstagramImage Load(InstagramFeed.Azure.InstagramImage image)
        {
            return new InstagramFeed.InstagramImage
            {
                caption = image.caption,
                created = image.created,
                imageLink = image.imageLink,
                instagramImageId = image.instagramImageId,
                link = image.link,
                thumbLink = image.thumbLink,
                userFullName = image.userFullName,
                userThumbnail = image.userThumbnail,
                userId = image.userId,
                longitude = image.longitude,
                latitude = image.latitude,
                username = image.username,
                isBlocked = image.isBlocked,
                votes = image.votes
            };
        }

        public IEnumerable<InstagramFeed.InstagramImage> GetPage(bool isAdmin, string search, string sortBy, int pageSize, int pageIndex)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureSettings.Instance.ConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("InstagramImage");

            // Create the CloudTable object that represents the "people" table.

            IEnumerable<InstagramFeed.Azure.InstagramImage> query;
            if (string.IsNullOrEmpty(search))
            {
                if (isAdmin)
                {
                    query = from entity in table.CreateQuery<InstagramFeed.Azure.InstagramImage>()
                        select entity ;
                }
                else {
                    query = from entity in table.CreateQuery<InstagramFeed.Azure.InstagramImage>()
                        where entity.isBlocked == false
                        select entity;
                }

            }
            else
            {
                if (isAdmin)
                {
                    query = from entity in table.CreateQuery<InstagramFeed.Azure.InstagramImage>()
                             where entity.userFullName.ToLower().Contains(search.ToLower())
                             select  entity;
                }
                else
                {
                    query = from entity in table.CreateQuery<InstagramFeed.Azure.InstagramImage>()
                        where entity.userFullName.ToLower().Contains(search.ToLower())
                            && entity.isBlocked == false
                        select entity ;
                }

            }

            List<InstagramImage> results;
            if (sortBy == "votes")
            {
                results = query.OrderByDescending(r => r.votes).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                results = query.OrderByDescending(r => r.created).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }

            List<InstagramFeed.InstagramImage> list = new List<InstagramFeed.InstagramImage>();

            foreach (var i in results)
                list.Add(this.Load(i));

            return list;
        }
    }
}
