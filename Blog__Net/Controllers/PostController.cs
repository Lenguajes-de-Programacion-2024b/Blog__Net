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
using MySql.Data.MySqlClient;
using Humanizer;


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
            // Obtener el IdUser desde los claims del usuario autenticado
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
                    // Usar las propiedades del objeto post
                    command.Parameters.AddWithValue("@Title", post.Title);
                    command.Parameters.AddWithValue("@Content", post.Content);
                    command.Parameters.AddWithValue("@Category", post.Category);
                    command.Parameters.AddWithValue("@PublicationDate", DateTime.Now); // Asumir la fecha actual o usar una propiedad si existe
                    command.Parameters.AddWithValue("@IdUser", idUser);
                    command.Parameters.AddWithValue("@Estado", post.Estado); // Asegúrate de que 'Estado' sea parte del modelo

                    command.ExecuteNonQuery();
                }
            }
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
            using (var connection = new MySqlConnection(_contexto.CadenaSQl)) // Cambiado a MySqlConnection
            {
                connection.Open();
                using (var command = new MySqlCommand("UpdatePost", connection)) // Cambiado a MySqlCommand
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Verificar si el usuario es un moderador
                    if (User.IsInRole("Moderador"))
                    {
                        // Los moderadores solo pueden actualizar el estado de la publicación
                        command.Parameters.AddWithValue("p_PostId", post.PostId); // Cambiado a p_PostId para MySQL
                        command.Parameters.AddWithValue("p_Title", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("p_Content", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("p_Category", DBNull.Value); // Pasar NULL si no se proporciona
                        command.Parameters.AddWithValue("p_Estado", post.Estado.ToString());
                    }
                    else if (User.IsInRole("Autor"))
                    {
                        // Los autores pueden actualizar todos los campos
                        command.Parameters.AddWithValue("p_PostId", post.PostId); // Cambiado a p_PostId para MySQL
                        command.Parameters.AddWithValue("p_Title", post.Title);
                        command.Parameters.AddWithValue("p_Content", post.Content);
                        command.Parameters.AddWithValue("p_Category", post.Category.ToString());
                        command.Parameters.AddWithValue("p_Estado", post.Estado.ToString()); // Si es necesario para autores
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
            using (var connection = new MySqlConnection(_contexto.CadenaSQl)) // Cambia a MySqlConnection
            {
                connection.Open();

                // Verificamos si el post tiene comentarios
                using (var command = new MySqlCommand("CheckPostComments", connection)) // Cambia a MySqlCommand
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_PostId", Id); // Asegúrate de que el nombre del parámetro coincida con el del procedimiento en MySQL

                    var hasComments = Convert.ToInt32(command.ExecuteScalar()); // Ejecutar y convertir el resultado a int
                    TempData["DebugHasComments"] = hasComments;

                    // Si tiene comentarios, no permitimos eliminar
                    if (hasComments > 0)
                    {
                        TempData["ErrorMessage"] = "No se puede eliminar un post que tiene comentarios.";
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Si no tiene comentarios, procedemos a eliminarlo
                using (var deleteCommand = new MySqlCommand("DeletePost", connection)) // Cambia a MySqlCommand
                {
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("p_PostId", Id); // Asegúrate de que el nombre del parámetro coincida
                    deleteCommand.ExecuteNonQuery();
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
            // Verificar que el comentario no esté vacío
            if (string.IsNullOrWhiteSpace(comment))
            {
                ViewBag.Error = "El comentario no puede estar vacío.";
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

                    // Asegúrate de que los nombres de los parámetros coincidan
                    cmd.Parameters.AddWithValue("p_Content", comment);
                    cmd.Parameters.AddWithValue("p_CreationDate", publicationDate);
                    cmd.Parameters.AddWithValue("p_IdUser", userId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_PostId", postId);
                    cmd.Parameters.AddWithValue("p_CommentParentId", commentparentId ?? (object)DBNull.Value);

                    // Abrir conexión y ejecutar el comando
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Redirigir de vuelta a la página de detalles del post
            return RedirectToAction("Details", "Post", new { id = postId });
        }




    }
}

