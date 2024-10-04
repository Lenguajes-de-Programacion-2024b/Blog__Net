using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;
using Blog__Net.Servicios.Contrato;
using Blog__Net.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Blog__Net.Data;
using Blog__Net.Data.ServicePost;


var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios necesarios
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbBlogContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("cadenaSQL"),
                     new MySqlServerVersion(new Version(8, 0, 23))));

// Registro de servicios personalizados
builder.Services.AddScoped<Contexto>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("cadenaSQL");
    return new Contexto(connectionString);
});

builder.Services.AddScoped<PostService>(); // Servicio para los posts
builder.Services.AddScoped<IInfoUserService, InfoUserService>(); // Servicio para la información del usuario

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// Configuración de caché para no almacenar respuestas
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None,
        }
    );
});

var app = builder.Build();

// Configura el pipeline de la aplicación
app.UseRouting();
app.UseStaticFiles(); // Para servir archivos estáticos (CSS, JS, etc.)
app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization(); // Habilita la autorización

// Configura las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Login}/{id?}");

app.Run();
