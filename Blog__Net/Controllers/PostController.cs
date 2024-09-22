using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
using Blog__Net.Models;
using Blog__Net.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using NuGet.Configuration;
using System.Data;

namespace Blog_.Net.Controllers
{
    public class PostController : Controller
    {
        private readonly Contexto _contexto;
        private readonly PostService _postservice;

        public PostController(Contexto con)
        {
            _contexto = con;
            _postservice = new PostService(con);
        }

        [Authorize(Roles = "Autor")]
        public IActionResult Create()
        {

            return View(new Posts());
        }

        [HttpPost]
        [Authorize(Roles = "Autor")]
        public IActionResult Create(Posts post)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("InsertPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Title", post.Title);
                    command.Parameters.AddWithValue("@Content", post.Content);
                    command.Parameters.AddWithValue("@Category", post.Category.ToString());
                    DateTime fc = DateTime.UtcNow;
                    command.Parameters.AddWithValue("@Publicationdate", fc);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Autor")]
        public ActionResult Update(int id)
        {
            var post = _postservice.GetpostbyId(id);
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "Autor")]
        public ActionResult Update(Posts post)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdatePost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", post.PostId);
                    command.Parameters.AddWithValue("@Title", post.Title);
                    command.Parameters.AddWithValue("@Content", post.Content);
                    command.Parameters.AddWithValue("@Category", post.Category.ToString());
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Autor")]
        public ActionResult Delete(Posts post)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("DeletePost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", post.PostId);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int id)
        {
            var post = _postservice.GetpostbyId(id);
            var comments = _postservice.ObtainCommentsByPostId(id);
            comments = _postservice.GetSonComment(comments);
            comments = _postservice.GetGrandSonComment(comments);

            var model = new PostDetailsViewModels
            {
                Post = post,
                MainComments = comments.Where(c => c.CommentparentId == null && c.CommentgrandparentId == null).ToList(),
                SonComments = comments.Where(c => c.CommentparentId != null && c.CommentgrandparentId != null).ToList(),
                GrandSonComments = comments.Where(c => c.CommentgrandparentId != null).ToList(),
                RecentPosts = _postservice.ObtainPosts().Take(10).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddComment(int postId, string comment, int? commentparentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comment))
                {
                    ViewBag.Error = "El comentario no puede estar vacio";
                    return RedirectToAction("Details", "Post", new { id = postId });
                }

                int? userId = null;
                var userIdClaim = User.FindFirst("IdUser");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;

                DateTime Publicationdate = DateTime.UtcNow;

                using (SqlConnection con = new(_contexto.CadenaSQl))
                {
                    using (SqlCommand cmd = new("AddComment", con)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Content",SqlDbType.VarChar).Value = comment;
                        cmd.Parameters.Add("@Creationdate", SqlDbType.DateTime2).Value = Publicationdate;
                        cmd.Parameters.Add("@PostId", SqlDbType.Int).Value = postId;
                        cmd.Parameters.Add("@IdUser", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@CommentparentId", SqlDbType.Int).Value = commentparentId ?? (object)DBNull.Value;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return RedirectToAction("Details", "Post", new { id = postId });
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return RedirectToAction("Details", "Post", new { id = postId });
            }
        }
    }
}