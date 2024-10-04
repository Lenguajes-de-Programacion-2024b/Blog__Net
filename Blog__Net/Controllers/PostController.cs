using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
using Blog__Net.Models;
using Blog__Net.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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
            var idUserClaim = User.FindFirst("IdUser");
            if (idUserClaim == null)
            {
                return Unauthorized();
            }

            int idUser = int.Parse(idUserClaim.Value);

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("InsertPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Title", post.Title);
                    command.Parameters.AddWithValue("@Content", post.Content);
                    command.Parameters.AddWithValue("@Category", post.Category);
                    command.Parameters.AddWithValue("@PublicationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@IdUser", idUser);
                    command.Parameters.AddWithValue("@Estado", post.Estado);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Autor, Moderador")]
        public ActionResult Update(int id)
        {
            var post = _postservice.GetPostById(id);
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "Autor, Moderador")]
        public ActionResult Update(Posts post)
        {
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("UpdatePost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("p_PostId", post.PostId);

                    if (User.IsInRole("Moderador"))
                    {
                        command.Parameters.AddWithValue("p_Title", DBNull.Value);
                        command.Parameters.AddWithValue("p_Content", DBNull.Value);
                        command.Parameters.AddWithValue("p_Category", DBNull.Value);
                        command.Parameters.AddWithValue("p_Estado", post.Estado.ToString());
                    }
                    else if (User.IsInRole("Autor"))
                    {
                        command.Parameters.AddWithValue("p_Title", post.Title);
                        command.Parameters.AddWithValue("p_Content", post.Content);
                        command.Parameters.AddWithValue("p_Category", post.Category.ToString());
                        command.Parameters.AddWithValue("p_Estado", post.Estado.ToString());
                    }

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Autor")]
        public ActionResult Delete(int Id)
        {
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();

                using (var command = new MySqlCommand("CheckPostComments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_PostId", Id);

                    var hasComments = Convert.ToInt32(command.ExecuteScalar());

                    if (hasComments > 0)
                    {
                        TempData["ErrorMessage"] = "No se puede eliminar un post que tiene comentarios.";
                        return RedirectToAction("Index", "Home");
                    }
                }

                using (var deleteCommand = new MySqlCommand("DeletePost", connection))
                {
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("p_PostId", Id);
                    deleteCommand.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int id)
        {
            var post = _postservice.GetPostById(id);
            var comments = _postservice.ObtainCommentsByPostId(id);
            comments = _postservice.GetSonComments(comments);
            comments = _postservice.GetGrandSonComments(comments);

            var model = new PostDetailsViewModels
            {
                Post = post,
                MainComments = comments.Where(c => c.CommentparentId == null && c.CommentgrandparentId == null).ToList(),
                SonComments = comments.Where(c => c.CommentparentId != null && c.CommentgrandparentId == null).ToList(),
                GrandSonComments = comments.Where(c => c.CommentgrandparentId != null).ToList(),
                RecentPosts = _postservice.ObtainPosts().Take(10).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddComment(int postId, string comment, int? commentparentId)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "El comentario no puede estar vacío.";
                return RedirectToAction("Details", "Post", new { id = postId });
            }

            int? userId = null;
            var userIdClaim = User.FindFirst("IdUser");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            DateTime publicationDate = DateTime.UtcNow;

            using (var con = new MySqlConnection(_contexto.CadenaSQl))
            {
                using (var cmd = new MySqlCommand("AddComment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_Content", comment);
                    cmd.Parameters.AddWithValue("p_CreationDate", publicationDate);
                    cmd.Parameters.AddWithValue("p_IdUser", userId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_PostId", postId);
                    cmd.Parameters.AddWithValue("p_CommentParentId", commentparentId ?? (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Details", "Post", new { id = postId });
        }
    }
}



           
