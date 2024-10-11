using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
using Blog__Net.Models;
using Blog__Net.Models.ViewModels;
using Blog__Net.Resources;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Configuration;
using System.Data;
using System.Security.Claims;

namespace Blog_.Net.Controllers
{
    public class PostController : Controller
    {
        private readonly Contexto _contexto;
        private readonly DbBlogContext _dbBlogContext;
        private readonly IPostLikesRepo postLikesRepo;
        private readonly PostService _postservice;

        public PostController(Contexto con, IPostLikesRepo postLikesRepo)
        {
            _contexto = con;
            this.postLikesRepo = postLikesRepo;
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
                // Manejo de error si no se encuentra el IdUser en las claims
                return Unauthorized();
            }
            int idUser = int.Parse(idUserClaim.Value);

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
                    command.Parameters.AddWithValue("@IdUser", idUser);
                    command.Parameters.AddWithValue("@Estado", post.Estado);
                    command.Parameters.AddWithValue("@likesCount", 0);
                    command.ExecuteNonQuery();
                }
            }

            // Inicializando el contador de likes a cero
            post.LikesCount = 0;

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Autor, Moderador")]
        public ActionResult Update(int id)
        {
            var post = _postservice.GetpostbyId(id);
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "Autor, Moderador")]
        public ActionResult Update(Posts post)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdatePost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Verificar si el usuario es un moderador
                    if (User.IsInRole("Moderador"))
                    {
                        // Los moderadores solo pueden actualizar el estado de la publicación
                        command.Parameters.AddWithValue("@PostId", post.PostId);
                        command.Parameters.AddWithValue("@Title", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("@Content", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("@Category", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("@Estado", post.Estado.ToString());
                    }
                    else if (User.IsInRole("Autor"))
                    {
                        // Los autores pueden actualizar todos los campos
                        command.Parameters.AddWithValue("@PostId", post.PostId);
                        command.Parameters.AddWithValue("@Title", post.Title);
                        command.Parameters.AddWithValue("@Content", post.Content);
                        command.Parameters.AddWithValue("@Category", post.Category.ToString());
                        command.Parameters.AddWithValue("@Estado", post.Estado.ToString()); // Si es necesario para autores
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
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();

                // Verificamos si el post tiene comentarios
                using (var command = new SqlCommand("CheckPostComments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", Id);

                    var hasComments = (int)command.ExecuteScalar();
                    TempData["DebugHasComments"] = hasComments;

                    // Si tiene comentarios, no permitimos eliminar
                    if (hasComments > 0)
                    {
                        // Podrías mostrar un mensaje de error o redirigir a una página específica
                        TempData["ErrorMessage"] = "No se puede eliminar un post que tiene comentarios.";
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Si no tiene comentarios, procedemos a eliminarlo
                using (var deleteCommand = new SqlCommand("DeletePost", connection))
                {
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("@PostId", Id);
                    deleteCommand.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Details(int id)
        {
            var post = _postservice.GetpostbyId(id);

            if (post == null) 
            { 
                return NotFound(); 
            }

            var comments = _postservice.ObtainCommentsByPostId(id);
            comments = _postservice.GetSonComment(comments);
            comments = _postservice.GetGrandSonComment(comments);

            var model = new PostDetailsViewModels
            {
                Post = post,
                LikesCount = post.LikesCount,
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

        [HttpPost]
        [Authorize(Roles = "Lector")]
        public async Task<ActionResult> LikePost(int postId, Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out userId))
                userId = Guid.Parse(userIdClaim.Value);

            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("AddLikeAndUpdateCount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", postId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    // Ejecutar el procedimiento
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return BadRequest("Ya le has dado like a este post.");
                        //TempData["LikeAlert"] = "Ya le has dado like a este post.";
                    }
                }
            }

            // Redirigir a la página de detalles del post
            return RedirectToAction("Details", new { id = postId });
        }



        [HttpPost]
        public async Task<ActionResult> UpdateLikesCount(int postId)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("IncrementLikesCount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", postId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Details", new { id = postId });
        }

            
    }
}