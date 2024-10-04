using Blog__Net.Controllers;
using Blog__Net.Models;
using MySql.Data.MySqlClient;  // Cambiado a MySQL
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

            // Cambiado SqlConnection a MySqlConnection
            using (MySqlConnection con = new(_contexto.CadenaSQl))
            {
                // Cambiado SqlCommand a MySqlCommand
                using (MySqlCommand cmd = new("GetUserById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUser", id);
                    con.Open();

                    // Cambiado SqlDataReader a MySqlDataReader
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        user = new InfoUser
                        {
                            IdUser = id,
                            UserName = rdr["UserName"].ToString(),
                            Email = rdr["Email"].ToString(),
                            Passcode = rdr["Passcode"].ToString(),
                            RolId = (int)rdr["RolId"]
                        };
                    }
                }
            }

            return user;
        }
    }
}

