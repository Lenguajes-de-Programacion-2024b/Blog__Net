using Blog__Net.Data.Enums;
using Blog__Net.Data.ServicePost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Moderador")]
public class ModeradorController : Controller
{
    private readonly PostService _postService;

    public ModeradorController(PostService postService)
    {
        _postService = postService;
    }

    public ActionResult RevisarContenidoInapropiado()
    {
        
        var postsInapropiados = _postService.ObtenerPostsInapropiados();

        
        return View(postsInapropiados);
    }
}

