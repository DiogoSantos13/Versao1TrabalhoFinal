using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Seed;
using Versao1TrabalhoFinal.Services;
using Versao1TrabalhoFinal.Cliente.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configuração da base de dados da aplicação.
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StandOficinaDB")));

// Configuração da cache em memória para suporte à sessão.
builder.Services.AddDistributedMemoryCache();

// Configuração da sessão da aplicação.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registo de serviços auxiliares do ASP.NET Core.
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Configuração do Identity com utilizadores e roles.
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<StandDbContext>()
    .AddDefaultTokenProviders();

// Configuração do cookie de autenticação.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
});

// Configuração das policies de autorização por perfis.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClienteOnly", policy =>
        policy.RequireRole("Cliente"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Mecanico", "Colaborador", "Vendedor", "Rececionista"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ProdutosAccess", policy =>
        policy.RequireRole("Cliente", "Admin", "Colaborador"));
});

// Configuração das Razor Pages e respetivas regras de acesso.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Registar");

    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Index");
    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Details");

    options.Conventions.AuthorizePage("/VeiculosStand/Create", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Edit", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Delete", "StaffOnly");

    options.Conventions.AuthorizeFolder("/ClienteArea", "ClienteOnly");
    options.Conventions.AuthorizeFolder("/Dashboard", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Produtos", "ProdutosAccess");
});

// Registo dos controllers com views, caso existam páginas MVC no projeto.
builder.Services.AddControllersWithViews();

// Registo dos serviços da aplicação.
builder.Services.AddScoped<OrcamentoService>();

// Registo dos serviços de integração com a API.
builder.Services.AddApiIntegration(builder.Configuration);

var app = builder.Build();

// Configuração do pipeline HTTP para ambiente não desenvolvimento.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware base da aplicação.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware de sessão. Deve ser chamado após o routing.
app.UseSession();

// Middleware de autenticação e autorização.
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento das Razor Pages.
app.MapRazorPages();

// Mapeamento de controllers MVC, caso existam.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Aplicar migrações e executar o seed inicial de roles e utilizadores.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<StandDbContext>();
        DbConnection? conn = db.Database.GetDbConnection();

        logger.LogInformation("Connection string (masked): {ConnectionString}", conn?.ConnectionString);

        try
        {
            if (conn != null)
            {
                await conn.OpenAsync();
                logger.LogInformation("DB connection aberta com sucesso.");
                await conn.CloseAsync();
            }
        }
        catch (SqlException sqlEx)
        {
            logger.LogError(
                sqlEx,
                "Falha ao abrir a ligação SQL (Number: {Number}, State: {State}): {Message}",
                sqlEx.Number,
                sqlEx.State,
                sqlEx.Message);

            throw;
        }
        catch (Exception openEx)
        {
            logger.LogError(openEx, "Erro ao abrir ligação à BD: {Message}", openEx.Message);
            throw;
        }

        await db.Database.MigrateAsync();
        await IdentitySeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro durante migração/seed: {Message}", ex.Message);
        throw;
    }
}

app.Run();