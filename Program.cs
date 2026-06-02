using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Tasks.Data;
using Tasks.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configurar Contexto de Base de Datos MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
});

// Configurar Identity para Usuarios y Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirige aquí si no ha iniciado sesión
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    // Le dice a .NET que confíe en el HTTPS que provee Render
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// --- INICIO: Crear base de datos y usuario administrador automáticamente ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    dbContext.Database.Migrate(); // Crea las tablas en Aiven automáticamente si no existen

    var adminExists = userManager.FindByEmailAsync("admin@jiraclone.com").Result;
    if (adminExists == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@jiraclone.com",
            Email = "admin@jiraclone.com",
            FullName = "Admin Principal"
        };
        userManager.CreateAsync(adminUser, "Admin123!").Wait(); // Esta será tu contraseña
    }
}
// --- FIN: Seeding ---

app.UseForwardedHeaders(); // <- Agregamos esto en lugar de HttpsRedirection
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <-- MUY IMPORTANTE: Debe ir antes de Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Index}/{id?}");

app.Run();
