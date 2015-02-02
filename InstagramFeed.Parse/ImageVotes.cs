using Parse.Api;
using Parse.Api.Models;

namespace InstagramFeed.Parse
{
    public class ImageVotes : IImageVotes
    {
        private readonly ParseRestClient _client;

        public ImageVotes() 
        {
            _client = new ParseRestClient(ParseSettings.Instance.ParseAppId,ParseSettings.Instance.ParseRestApiKey);
        }

        public void Initialize()
        {
            // Do nothing
        }

        public void Create(InstagramFeed.ImageVote image) 
        {
            InstagramFeed.Parse.ImageVote persistedVote = new InstagramFeed.Parse.ImageVote
            {
                instagramImageId = image.instagramImageId,
                voterId = image.voterId
            };
            _client.CreateObject(persistedVote);
        }

        public InstagramFeed.ImageVote GetVote(string voter, string instagramImageId)
        {
            QueryResult<InstagramFeed.Parse.ImageVote> result = _client.GetObjects<InstagramFeed.Parse.ImageVote>(new { voterId = voter, instagramImageId = instagramImageId });
            if (result == null || result.Results == null ||result.Results.Count == 0)
                return null;
            return this.Load(result.Results[0]);
        }

        public int GetVoteCount(string instagramImageId)
        {
            return _client.GetObjects<InstagramFeed.Parse.ImageVote>(new { instagramImageId = instagramImageId }).TotalCount;
        }

        private InstagramFeed.ImageVote Load(InstagramFeed.Parse.ImageVote vote) 
        {
            return new InstagramFeed.ImageVote
            {
                instagramImageId = vote.instagramImageId,
                voterId = vote.voterId
            };
        }

    }
}
