using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream
using Blog__Net.Models;

=======
using Blog__Net.Servicios.Contrato;
using Blog__Net.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Blog__Net.Data;
using Blog__Net.Data.ServicePost;
>>>>>>> Stashed changes

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

<<<<<<< Updated upstream
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

=======
// Configuración del contexto de la base de datos
builder.Services.AddDbContext<DbBlogContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("cadenaSQL"),
                     new MySqlServerVersion(new Version(8, 0, 23))));

// Inicializa el contexto y el servicio PostService
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
>>>>>>> Stashed changes
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
