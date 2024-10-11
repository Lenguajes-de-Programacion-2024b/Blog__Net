using Blog__Net.Data.ServicePost;
using Blog__Net.Data;
using Blog__Net.Models;
using Microsoft.AspNetCore.Mvc;

using Blog__Net.Data.Enums;
using X.PagedList;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Blog__Net.Resources;
using Microsoft.Extensions.Hosting;

namespace Blog__Net.Controllers;


public class HomeController : Controller
{
    private readonly Contexto _contexto;
    private readonly PostService _postservice;
    private IPostLikesRepo postLikesRepo;

    public HomeController(Contexto contexto, IPostLikesRepo postLikesRepo)
    {
        _contexto = contexto;
        _postservice = new PostService(contexto);
        this.postLikesRepo = postLikesRepo;
    }

    public async Task<IActionResult> Index(string category, string search, DateTime? publicationDate, int? page)
    {
        var posts = new List<Posts>();

        if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(search) && publicationDate == null)
        {
            posts = _postservice.ObtainPosts();
        }
        else if (!string.IsNullOrEmpty(category))
        {
            var categoriaEnum = Enum.Parse<CategoriaEnum>(category);
            posts = _postservice.ObtainPostsByCategory(categoriaEnum);

            if (posts.Count == 0)
                ViewBag.Error = $"No se encontraron publicaciones en la categoría {categoriaEnum}.";
            
        }
        else
        {
            // Búsqueda por palabras clave en título, nombre de usuario, y/o fecha
            posts = _postservice.ObtainPostsByFilter(search, publicationDate);

            if (posts.Count == 0)
                ViewBag.Error = $"No se encontraron publicaciones con los criterios proporcionados.";
        }

        int pageSize = 6;
        int pageNumber = (page ?? 1);

        string categoryDescription = !string.IsNullOrEmpty(category)
            ? CategoriaEnumHelper.ObtainDescription(Enum.Parse<CategoriaEnum>(category))
            : "Todas las demás";

        ViewBag.CategoriaDescription = categoryDescription;

        return View(posts.ToPagedList(pageNumber, pageSize));
    }

    public async Task<IActionResult> logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Inicio");
    }
}
