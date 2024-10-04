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

        public Posts GetpostbyId(int id)
        {
            var post = new Posts();
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("getpostbyId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), reader.GetString("Category")),
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

        public List<Posts> ObtainPosts()
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("GetAllPost", connection)) // Asegúrate de que el nombre del procedimiento almacenado sea correcto
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
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), reader.GetString("Category")),
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

        public List<Posts> ObtainPostsByCategory(CategoriaEnum categoria)
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("getpostbycategory", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_Category", categoria.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), reader.GetString("Category")),
                                Publicationdate = reader.GetDateTime("Publicationdate")
                            };
                            posts.Add(post);
                        }
                    }
                }
            }

            return posts;
        }

        public List<Posts> ObtainPostsByFilter(string search = null, DateTime? publicationDate = null)
        {
            var posts = new List<Posts>();

            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("getPostsByFilter", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PublicationDate", (object)publicationDate?.Date ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = reader.GetInt32("PostId"),
                                Title = reader.GetString("Title"),
                                Content = reader.GetString("Content"),
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), reader.GetString("Category")),
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

        public List<Comments> ObtainCommentsByPostId(int id)
        {
            var comments = new List<Comments>();
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new MySqlCommand("ObtainCommentsByPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_PostId", id); // Cambia "@PostId" por "p_PostId"
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


        public List<Comments> GetSonComment(List<Comments> comments)
        {
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    using (var command = new MySqlCommand("GetSonCommentsbyCommentId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_CommentId", comment.CommentId);
                        using (var reader = command.ExecuteReader())
                        {
                            var soncomments = new List<Comments>();
                            while (reader.Read())
                            {
                                var soncomment = new Comments
                                {
                                    CommentId = reader.GetInt32("CommentId"),
                                    Content = reader.GetString("Content"),
                                    Creationdate = reader.GetDateTime("Creationdate"),
                                    IdUser = reader.GetInt32("IdUser"),
                                    PostId = reader.GetInt32("PostId"),
                                    UserName = reader.GetString("UserName"),
                                    CommentparentId = comment.CommentId
                                };
                                soncomments.Add(soncomment);
                            }
                            comment.SonComments = soncomments;
                        }
                    }
                }
            }
            return comments;
        }

        public List<Comments> GetGrandSonComment(List<Comments> comments)
        {
            using (var connection = new MySqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    if (comment.SonComments is not null)
                    {
                        foreach (var soncomment in comment.SonComments)
                        {
                            using (var command = new MySqlCommand("GetSonCommentsbyCommentId", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@CommentId", soncomment.CommentId);
                                using (var reader = command.ExecuteReader())
                                {
                                    var grandsoncomments = new List<Comments>();
                                    while (reader.Read())
                                    {
                                        var grandsoncomment = new Comments
                                        {
                                            CommentId = reader.GetInt32("CommentId"),
                                            Content = reader.GetString("Content"),
                                            Creationdate = reader.GetDateTime("Creationdate"),
                                            IdUser = reader.GetInt32("IdUser"),
                                            PostId = reader.GetInt32("PostId"),
                                            UserName = reader.GetString("UserName"),
                                            CommentparentId = soncomment.CommentId,
                                            CommentgrandparentId = comment.CommentId
                                        };
                                        grandsoncomments.Add(grandsoncomment);
                                    }
                                    soncomment.SonComments = grandsoncomments;
                                }
                            }
                        }
                    }
                }
            }
            return comments;
        }
    }
}
