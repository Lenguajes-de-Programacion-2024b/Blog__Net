using Blog__Net.Data.ServicePost;
using Blog__Net.Data;
using Blog__Net.Models;
using Microsoft.AspNetCore.Mvc;

using Blog__Net.Data.Enums;
using X.PagedList;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Blog__Net.Controllers;


public class HomeController : Controller
{
    private readonly Contexto _contexto;
    private readonly PostService _postservice;

    public HomeController(Contexto contexto)
    {
        _contexto = contexto;
        _postservice = new PostService(contexto);
    }

    public IActionResult Index(string category, string search, int? page)
    {
        var post = new List<Posts>();
        if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(search))
            post = _postservice.ObtainPosts();

        else if (!string.IsNullOrEmpty(category))
        {
            var categoriaEnum=Enum.Parse<CategoriaEnum>(category);
            post=_postservice.ObtainPostsByCategory(categoriaEnum);

            if (post.Count == 0)
                ViewBag.Error = $"No se encontraron publicaciones en la categoria {categoriaEnum}.";
        }
        else if (string.IsNullOrEmpty(search))
        {
            post=_postservice.ObtainPostsByTitle(search);
            if(post.Count == 0)
                ViewBag.Error = $"No se encontraron publicaciones en la categoria {search}.";
        }

        int pageSize = 6;
        int pageNumber = (page ?? 1);

        string categorydescription = !string.IsNullOrEmpty(category) ? CategoriaEnumHelper.ObtainDescription(Enum.Parse<CategoriaEnum>(category)):"Todas las demás";
        ViewBag.CategoriaDescription = categorydescription;

        return View(post.ToPagedList(pageNumber, pageSize));
    }

    public async Task<IActionResult> logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Inicio");
    }
}
