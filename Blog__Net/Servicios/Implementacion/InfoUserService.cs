using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;
using Blog__Net.Servicios.Contrato;

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

        public async Task<InfoUser> SaveInfoUser(InfoUser modelo)
        {
            _dbContext.InfoUsers.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
