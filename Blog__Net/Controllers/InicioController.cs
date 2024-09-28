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
        public async Task<IActionResult> Registered(InfoUser modelo)
        {
            // Validación de requisitos de la contraseña
            bool mayuscula = false, minuscula = false, numero = false, carespecial = false;
            string passcode = modelo.Passcode;

            for (int i = 0; i < passcode.Length; i++)
            {
                if (char.IsUpper(passcode[i]))
                {
                    mayuscula = true;
                }
                else if (char.IsLower(passcode[i]))
                {
                    minuscula = true;
                }
                else if (char.IsDigit(passcode[i]))
                {
                    numero = true;
                }
                else
                {
                    carespecial = true;
                }
            }

            // Validación de la contraseña
            if (!(mayuscula && minuscula && numero && carespecial && passcode.Length >= 8))
            {
                ViewData["Mensaje1"] = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial.";
                return View(modelo);
            }

            // Verificación de que las contraseñas coincidan
            if (modelo.Passcode != modelo.ConfirmPasscode)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden.";
                return View(modelo);
            }

            // Verificar si el usuario o el correo ya existen
            bool userExists = await _infoUserService.UserExists(modelo.UserName, modelo.Email);
            if (userExists)
            {
                ViewData["Mensaje"] = "Usuario o Correo ya existe";
                return View(modelo);
            }

            // Encriptar contraseña
            modelo.Passcode = Utilidades.EncriptarClave(modelo.Passcode);

            // Guardar el usuario en la base de datos
            InfoUser created_user = await _infoUserService.SaveInfoUser(modelo);

            // Si el usuario fue creado correctamente
            if (created_user.IdUser > 0)
            {
                return RedirectToAction("Login", "Inicio");
            }

            // Mostrar mensaje de error si no se pudo crear el usuario
            ViewData["Message"] = "No se pudo crear el usuario";
            return View(modelo);
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
                ViewBag.Message = "Correo o Contraseña incorrectos";
                return View();
            }

                    // Obtener el nombre del rol basado en rolId
            string roleName = null;
            if (user_found.RolId.HasValue)
            {
                roleName = await _infoUserService.GetRoleNameById(user_found.RolId.Value);
            }

            if (string.IsNullOrEmpty(roleName))
            {
                ViewBag.Message = "El usuario no tiene un rol asignado.";
                return View();
            }

            // Autenticación exitosa: Añadir Claims (Nombre y Rol)
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user_found.UserName),
                new Claim(ClaimTypes.Role, roleName), // Usar el nombre del rol
                new Claim("IdUser", user_found.IdUser.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
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
