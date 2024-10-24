
using Blog__Net.Data;
using Blog__Net.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Versioning;

namespace Blog__Net.Resources
{
    public class PostLikesRepo : IPostLikesRepo
    {
        private readonly DbBlogContext _dbBlogContext;
        private readonly Contexto _contexto;
        private readonly Posts post;

        public PostLikesRepo(DbBlogContext dbContext)
        {
            _dbBlogContext = dbContext;
        }

        // Método para obtener los likes de un post
        public async Task<int> GetPostLikesBypostId(int postId)
        {
            var likesTotal = _dbBlogContext.Posts.Where(p => p.PostId == postId);
            return likesTotal.Count();
        }

        public Task<int> GetTotalLikes(int postId, Posts post)
        {
            return GetTotalLikes(postId);
        }

        // Implementación del método para obtener el total de likes
        public async Task<int> GetTotalLikes(int postId)
        {
            return post.LikesCount;
        }

        // Método para agregar un like a una publicación
        public async Task<bool> AddLike(int userId, int postId)
        {
            var existingLike = await _dbBlogContext.PostLike
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PostId == postId);

            if (existingLike == null)
            {
                var newLike = new PostLike
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PostId = postId
                };

                _dbBlogContext.PostLike.Add(newLike);
                await _dbBlogContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // Método para eliminar un like (anteriormente implementado)
        public async Task<bool> RemoveLike(int userId, int postId)
        {
            var existingLike = await _dbBlogContext.PostLike
                .FirstOrDefaultAsync(p => p.UserId == userId && p.PostId == postId);

            if (existingLike != null)
            {
                _dbBlogContext.PostLike.Remove(existingLike);
                await _dbBlogContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        Task<PostLike> IPostLikesRepo.GetLikeForPostByUser(int postId, Guid userId)
        {
            throw new NotImplementedException();
        }

        Task IPostLikesRepo.AddLike(PostLike like)
        {
            throw new NotImplementedException();
        }

        Task<List<PostLike>> IPostLikesRepo.GetPostsLikedByUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        Task<List<PostLike>> IPostLikesRepo.GetPostLikesBypostId(int postId)
        {
            throw new NotImplementedException();
        }
    }
}
