using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Repositories;
using Sistema.RENACER.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + Newtonsoft
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

// EF Core — SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<RenacerDbContext>(opt =>
    opt.UseSqlServer(connectionString));

// Autenticación con Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Cuenta/Login";
        opt.LogoutPath = "/Cuenta/Logout";
        opt.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.Cookie.Name = "RENACER.Auth";
    });

builder.Services.AddAuthorization();

// IMemoryCache para bloqueo de intentos y tokens de recuperación
builder.Services.AddMemoryCache();

// HttpContextAccessor para acceder al IP en servicios
builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();

// Services
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IRecuperacionService, RecuperacionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto → Login (si no autenticado, redirige automáticamente vía cookie)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();
