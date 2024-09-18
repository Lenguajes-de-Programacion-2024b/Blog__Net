using Microsoft.AspNetCore.Mvc;

using Blog__Net.Models;
using Blog__Net.Resources;
using Blog__Net.Servicios.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Numerics;

namespace Blog__Net.Controllers
{
    public class InicioController : Controller
    {
        private readonly IInfoUserService _infoUserService;

        public InicioController(IInfoUserService infoUserService)
        {
            _infoUserService = infoUserService;
        }
        public IActionResult Registered()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Registered(InfoUser modelo)
        {
            modelo.Passcode = Utilidades.EncriptarClave(modelo.Passcode);

            InfoUser created_user = await _infoUserService.SaveInfoUser(modelo);

            if (created_user.IdUser > 0)
                return RedirectToAction("Login", "Inicio");

            ViewData["Message"] = "No se pudo crear el usuario";
            return View();
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(string Email, string Passcode)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Passcode))
            {
                ViewBag.Message = "El correo y la contraseña son obligatorios.";
                return View();
            }

            // Encriptar la contraseña y buscar al usuario
            InfoUser user_found = await _infoUserService.GetInfoUser(Email, Utilidades.EncriptarClave(Passcode));

            if (user_found == null)
            {
                ViewBag.Message = "No se encontraron coincidencias";
                return View();
            }

            // Autenticación exitosa
            List<Claim> claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, user_found.UserName)
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }
    }
}
