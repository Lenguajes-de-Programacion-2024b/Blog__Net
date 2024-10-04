using Blog__Net.Data.Enums;
using Blog__Net.Models;
using MySqlConnector;
using System.Data;

namespace Blog__Net.Data.ServicePost
{
    public class PostService
    {
        private readonly Contexto _contexto;

        public PostService(Contexto con)
        {
            _contexto = con;
        }

        
        public Posts GetPostById(int id)
        {
            Posts post = null;

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("getPostById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_PostId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = Enum.Parse<CategoriaEnum>(reader.GetString("Category")),
                                Publicationdate = reader.GetDateTime("Publicationdate"),
                                UserName = reader.GetString("UserName"),
                                IdUser = reader.GetInt32("IdUser")
                            };
                        }
                    }
                }
            }
            return post;
        }

        // Obtiene todos los posts
        public List<Posts> ObtainPosts()
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("GetAllPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = Enum.Parse<CategoriaEnum>(reader.GetString("Category")),
                                Publicationdate = reader.GetDateTime("Publicationdate"),
                                UserName = reader.GetString("UserName")
                            };
                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }

        // Obtiene posts por categoría
        public List<Posts> ObtainPostsByCategory(CategoriaEnum categoria)
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("getpostbycategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_Category", categoria.ToString());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = Enum.Parse<CategoriaEnum>(reader.GetString("Category")),
                                Publicationdate = reader.GetDateTime("Publicationdate")
                            };
                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }

        // Obtiene posts filtrados por búsqueda y fecha de publicación
        public List<Posts> ObtainPostsByFilter(string search = null, DateTime? publicationDate = null)
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("getPostsByFilter", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PublicationDate", (object)publicationDate?.Date ?? DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = Enum.Parse<CategoriaEnum>(reader.GetString("Category")),
                                Publicationdate = reader.GetDateTime("Publicationdate"),
                                UserName = reader.GetString("UserName")
                            };
                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }

        // Obtiene comentarios por ID del post
        public List<Comments> ObtainCommentsByPostId(int postId)
        {
            var comments = new List<Comments>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("ObtainCommentsByPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_PostId", postId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var comment = new Comments
                            {
                                CommentId = reader.GetInt32("CommentId"),
                                Content = reader.GetString("Content"),
                                Creationdate = reader.GetDateTime("Creationdate"),
                                IdUser = reader.GetInt32("IdUser"),
                                PostId = reader.GetInt32("PostId"),
                                UserName = reader.GetString("UserName")
                            };
                            comments.Add(comment);
                        }
                    }
                }
            }

            return comments;
        }

        // Obtiene comentarios hijos (directos) de un comentario
        public List<Comments> GetSonComments(List<Comments> comments)
        {
            foreach (var comment in comments)
            {
                comment.SonComments = GetCommentsByParentId(comment.CommentId);
            }
            return comments;
        }

        // Obtiene comentarios nietos (indirectos) de un comentario
        public List<Comments> GetGrandSonComments(List<Comments> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.SonComments != null)
                {
                    foreach (var soncomment in comment.SonComments)
                    {
                        soncomment.SonComments = GetCommentsByParentId(soncomment.CommentId);
                    }
                }
            }
            return comments;
        }

        // Función reutilizable para obtener comentarios por ID de comentario padre
        private List<Comments> GetCommentsByParentId(int parentId)
        {
            var comments = new List<Comments>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("GetSonCommentsByCommentId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_CommentId", parentId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var comment = new Comments
                            {
                                CommentId = reader.GetInt32("CommentId"),
                                Content = reader.GetString("Content"),
                                Creationdate = reader.GetDateTime("Creationdate"),
                                IdUser = reader.GetInt32("IdUser"),
                                PostId = reader.GetInt32("PostId"),
                                UserName = reader.GetString("UserName"),
                                CommentparentId = parentId
                            };
                            comments.Add(comment);
                        }
                    }
                }
            }

            return comments;
        }
    }
}
