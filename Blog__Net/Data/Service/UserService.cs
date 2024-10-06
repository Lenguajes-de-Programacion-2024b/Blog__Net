using Blog__Net.Controllers;
using Blog__Net.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Blog__Net.Data.ServicePost
{
    public class UserService
    {
        private readonly Contexto _contexto;

        public UserService(Contexto con)
        {
            _contexto = con;
        }

        public InfoUser GetUserById(int id)
        {
            InfoUser user = new();

            using (SqlConnection con = new(_contexto.CadenaSQl))
            {
                using(SqlCommand cmd= new("GetUserById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUser",id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        user = new InfoUser
                        {
                            IdUser = id,
                            UserName = rdr["UserName"].ToString(),
                            Email= rdr["Email"].ToString(),
                            Passcode= rdr["Passcode"].ToString(),
                            RolId = (int)rdr["RolId"]
                        };
                    }
                }
            }

            return user;
        }
    }
}
