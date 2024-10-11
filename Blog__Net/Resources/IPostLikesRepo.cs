using Blog__Net.Models;

namespace Blog__Net.Resources
{
    public interface IPostLikesRepo
    {

        Task<int> GetTotalLikes(int postId);
        Task<List<PostLike>> GetPostLikesBypostId(int postId);
        Task<PostLike> GetLikeForPostByUser(int postId, Guid userId);
        Task AddLike(PostLike like);
        Task<List<PostLike>> GetPostsLikedByUser(Guid userId);
    }
}
