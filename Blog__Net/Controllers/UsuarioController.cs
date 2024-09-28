using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
using Blog__Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog__Net.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Contexto _contexto;
        private readonly UserService _userService;

        public UsuarioController(Contexto contexto)
        {
            _contexto = contexto;
            _userService=new UserService(contexto);
        }

        public ActionResult Profile()
        {
            int userId = 0;
            var userIdClaim = User.FindFirst("IdUser");
            if(userIdClaim !=null && int.TryParse(userIdClaim.Value, out int parseUserId))
                userId = parseUserId;

            InfoUser user= _userService.GetUserById(userId);
            return View();
        }
    }
}
