using Microsoft.EntityFrameworkCore;
using Blog__Net.Models;
using Blog__Net.Servicios.Contrato;
using Blog__Net.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
using Blog__Net.Servicios.Contrato;
using Blog__Net.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Blog__Net.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();


app.UseRouting();


builder.Services.AddDbContext<DbBlogContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("cadenaSQL"),
                     new MySqlServerVersion(new Version(8, 0, 23))));


builder.Services.AddScoped<Contexto>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("cadenaSQL");
    return new Contexto(connectionString);
});
builder.Services.AddScoped<PostService>(); // Registra el servicio

builder.Services.AddScoped<IInfoUserService, InfoUserService>();

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

app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Login}/{id?}");


app.Run();

