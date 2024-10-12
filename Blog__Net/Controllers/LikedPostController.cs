using Blog__Net.Data.ServicePost;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog__Net.Controllers
{
    public class LikedPostController : Controller
    {

        private readonly PostService _postService;

        public LikedPostController(PostService postService)
        {
            _postService = postService;
        }

        public ActionResult LikedPosts()
        {
            int? userId = null;
            var userIdClaim = User.FindFirst("IdUser");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                userId = parsedUserId;
            var likedPosts = _postService.ObtainLikedPostsByUser(userId);
            return View(likedPosts);
        }
    }
}
