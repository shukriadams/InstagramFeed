namespace InstagramFeed
{
    public interface IImageVotes
    {
        void Initialize();

        /// <summary>
        /// 
        /// </summary>
        ImageVote GetVote(string voter, string instagramImageId);

        /// <summary>
        /// 
        /// </summary>
        void Create(ImageVote image);

        /// <summary>
        /// 
        /// </summary>
        int GetVoteCount(string instagramImageId);
    }
}
