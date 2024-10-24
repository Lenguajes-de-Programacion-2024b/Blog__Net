using Microsoft.EntityFrameworkCore;

using Blog__Net.Servicios.Contrato;
using Blog__Net.Servicios.Implementacion;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Blog__Net.Data;
using Blog__Net.Resources;
using Blog__Net.Data.ServicePost; // Asegúrate de tener este using para PostService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(new Contexto(builder.Configuration.GetConnectionString("cadenaSQL")));

builder.Services.AddDbContext<DbBlogContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL"));
});

// Registrar los servicios necesarios
builder.Services.AddScoped<IInfoUserService, InfoUserService>();
builder.Services.AddScoped<IPostLikesRepo, PostLikesRepo>();

// Asegúrate de registrar PostService
builder.Services.AddScoped<PostService>(); // Agregar esta línea

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

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

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Login}/{id?}");

app.Run();
