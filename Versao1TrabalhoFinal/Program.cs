using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Seed;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adiciona suporte a Razor Pages.
/// </summary>
builder.Services.AddRazorPages();

/// <summary>
/// Regista o DbContext da aplicação.
/// </summary>
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("StandOficinaDB")));

/// <summary>
/// Configura o Identity com utilizadores e roles.
/// </summary>
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<StandDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Configura as políticas de autorização por role.
/// </summary>
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Mecanica", policy =>
        policy.RequireRole("Admin", "Mecanico"));

    options.AddPolicy("Comercial", policy =>
        policy.RequireRole("Admin", "Vendedor"));

    options.AddPolicy("Rececionista", policy =>
        policy.RequireRole("Admin", "Rececionista"));

    options.AddPolicy("Staff", policy =>
        policy.RequireRole("Admin", "Mecanico", "Vendedor", "Rececionista"));

    options.AddPolicy("ClienteOnly", policy =>
        policy.RequireRole("Cliente"));
});

/// <summary>   
/// Isto ajuda a ter um fluxo mais profissional quando um utilizador tenta
/// aceder a páginas protegidas sem permissão
/// </summary>

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/AccessDenied";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

/// <summary>
/// Executa o seed de roles e utilizadores iniciais.
/// </summary>
using (var scope = app.Services.CreateScope())
{
   var services = scope.ServiceProvider;
   await IdentitySeeder.SeedAsync(services);
}

app.Run();
