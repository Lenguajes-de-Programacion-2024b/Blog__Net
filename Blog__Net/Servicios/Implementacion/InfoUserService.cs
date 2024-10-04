using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;
using Blog__Net.Servicios.Contrato;
using Blog__Net.Data;

namespace Blog__Net.Servicios.Implementacion
{
    public class InfoUserService : IInfoUserService
    {
        private readonly DbBlogContext _dbContext;

        public InfoUserService(DbBlogContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InfoUser> GetInfoUser(string Email, string Passcode)
        {
            InfoUser user_found = await _dbContext.InfoUsers.Where(u => u.Email == Email && u.Passcode == Passcode)
                .FirstOrDefaultAsync();

            return user_found;
        }
        public async Task<string> GetRoleNameById(int rolId)
        {
            var role = await _dbContext.Roles.FindAsync(rolId);
            return role?.RolName; // Devuelve el nombre del rol o null si no se encuentra
        }

        // Método para verificar si un usuario o correo ya existe
        public async Task<bool> UserExists(string userName, string email)
        {
            return await _dbContext.InfoUsers
                .AnyAsync(u => u.UserName == userName || u.Email == email);
        }

        public async Task<InfoUser> SaveInfoUser(InfoUser modelo)
        {
            _dbContext.InfoUsers.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}