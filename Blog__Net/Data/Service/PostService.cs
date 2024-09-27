using Blog__Net.Data.Enums;
using Blog__Net.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
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
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("getpostbyId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Posts
                            {
                                PostId = (int)reader["PostId"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Category"]),
                                Publicationdate = (DateTime)reader["Publicationdate"],
                                UserName = (string)reader["UserName"],
                                IdUser = (int)reader["IdUser"]
                            };
                        }
                        reader.Close();
                    }
                }
            }
            return post;
        }

        public List<Posts> ObtainPosts()
        {
            var posts = new List<Posts>();

            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (SqlCommand cmd = new("GetallPost", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = (int)reader["PostId"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Category"]),
                                Publicationdate = (DateTime)reader["Publicationdate"]
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

            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (SqlCommand cmd = new("getpostbycategory", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Category", categoria.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Posts
                            {
                                PostId = (int)reader["PostId"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Category"]),
                                Publicationdate = (DateTime)reader["Publicationdate"]
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

            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("getPostsByFilter", connection))
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
                                PostId = (int)reader["PostId"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                Category = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Category"]),
                                Publicationdate = (DateTime)reader["Publicationdate"],
                                UserName = (string)reader["UserName"]
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
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtainCommentsByPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var comment = new Comments
                            {
                                CommentId = (int)reader["CommentId"],
                                Content = (string)reader["Content"],
                                Creationdate = (DateTime)reader["Creationdate"],
                                IdUser = (int)reader["IdUser"],
                                PostId = (int)reader["PostId"],
                                UserName = (string)reader["UserName"]
                            };
                            comments.Add(comment);
                        }
                        reader.Close();
                    }
                }
            }

            return comments;
        }

        public List<Comments> GetSonComment(List<Comments> comments)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    using (var command = new SqlCommand("GetSonCommentsbyCommentId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CommentId", comment.CommentId);
                        using (var reader = command.ExecuteReader())
                        {
                            var soncomments = new List<Comments>();
                            while (reader.Read())
                            {
                                var soncomment = new Comments
                                {
                                    CommentId = (int)reader["CommentId"],
                                    Content = (string)reader["Content"],
                                    Creationdate = (DateTime)reader["Creationdate"],
                                    IdUser = (int)reader["IdUser"],
                                    PostId = (int)reader["PostId"],
                                    UserName = (string)reader["UserName"],
                                    CommentparentId = comment.CommentId
                                };
                                soncomments.Add(soncomment);
                            }
                            reader.Close();
                            comment.SonComments = soncomments;
                        }
                    }
                }
            }
            return comments;
        }

        public List<Comments> GetGrandSonComment(List<Comments> comments)
        {
            using (var connection = new SqlConnection(_contexto.CadenaSQl))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    if (comment.SonComments is not null)
                    {
                        foreach(var soncomment in comment.SonComments)
                        {
                            using (var command = new SqlCommand("GetSonCommentsbyCommentId", connection))
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
                                            CommentId = (int)reader["CommentId"],
                                            Content = (string)reader["Content"],
                                            Creationdate = (DateTime)reader["Creationdate"],
                                            IdUser = (int)reader["IdUser"],
                                            PostId = (int)reader["PostId"],
                                            UserName = (string)reader["UserName"],
                                            CommentparentId = soncomment.CommentId,
                                            CommentgrandparentId = comment.CommentId
                                        };
                                        grandsoncomments.Add(grandsoncomment);
                                    }
                                    reader.Close();
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
