using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Versao1TrabalhoFinal.Cliente.Extensions;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Seed;
using Versao1TrabalhoFinal.Services;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração da base de dados da aplicação.
/// Usa SQL Server com a connection string definida no appsettings.json.
/// </summary>
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StandOficinaDB")));

/// <summary>
/// Configuração da cache em memória.
/// Necessária para suporte à sessão.
/// </summary>
builder.Services.AddDistributedMemoryCache();

/// <summary>
/// Configuração da sessão da aplicação.
/// Define o tempo limite e propriedades do cookie de sessão.
/// </summary>
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/// <summary>
/// Registo de serviços auxiliares do ASP.NET Core.
/// </summary>
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

/// <summary>
/// Configuração do ASP.NET Core Identity com utilizadores e roles.
/// As passwords estão simplificadas para ambiente académico/desenvolvimento.
/// </summary>
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

/// <summary>
/// Configuração do cookie de autenticação.
/// Define páginas de login, logout e acesso negado.
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
});

/// <summary>
/// Configuração das policies de autorização por perfis.
/// Estas policies podem ser usadas em Razor Pages ou controllers.
/// </summary>
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClienteOnly", policy =>
        policy.RequireRole("Cliente"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Mecanico", "Colaborador", "Vendedor", "Rececionista", "Funcionario"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ProdutosAccess", policy =>
        policy.RequireRole("Cliente", "Admin", "Colaborador"));

    options.AddPolicy("ColaboradoresView", policy =>
        policy.RequireRole("Admin", "Colaborador"));
});

/// <summary>
/// Configuração das Razor Pages e das regras de acesso por página.
/// </summary>
builder.Services.AddRazorPages(options =>
{
    /// <summary>
    /// Páginas públicas.
    /// </summary>
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Registar");
    options.Conventions.AllowAnonymousToPage("/Contactos");

    /// <summary>
    /// Área interna do staff.
    /// </summary>
    options.Conventions.AuthorizeFolder("/PainelEmpresa", "StaffOnly");

    /// <summary>
    /// Páginas públicas do stand.
    /// </summary>
    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Index");
    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Details");

    /// <summary>
    /// Gestão de viaturas do stand: apenas staff.
    /// </summary>
    options.Conventions.AuthorizePage("/VeiculosStand/Create", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Edit", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Delete", "StaffOnly");

    /// <summary>
    /// Pasta reservada a clientes.
    /// </summary>
    options.Conventions.AuthorizeFolder("/ClienteArea", "ClienteOnly");

    /// <summary>
    /// Dashboard reservado a administradores.
    /// </summary>
    options.Conventions.AuthorizeFolder("/Dashboard", "AdminOnly");

    /// <summary>
    /// Produtos acessíveis conforme a policy definida.
    /// </summary>
    options.Conventions.AuthorizeFolder("/Produtos", "ProdutosAccess");

    /// <summary>
    /// Colaboradores:
    /// - Index pode ser visto por Admin e Colaborador
    /// - Create/Edit/Delete apenas por Admin
    /// </summary>
    options.Conventions.AuthorizePage("/Colaboradores/Index", "ColaboradoresView");
    options.Conventions.AuthorizePage("/Colaboradores/Create", "AdminOnly");
    options.Conventions.AuthorizePage("/Colaboradores/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/Colaboradores/Delete", "AdminOnly");
});

/// <summary>
/// Registo de controllers com views, caso existam componentes MVC no projeto.
/// </summary>
builder.Services.AddControllersWithViews();

/// <summary>
/// Registo de serviços próprios da aplicação.
/// </summary>
builder.Services.AddScoped<OrcamentoService>();

/// <summary>
/// Registo dos serviços de integração com APIs externas.
/// </summary>
builder.Services.AddApiIntegration(builder.Configuration);

var app = builder.Build();

/// <summary>
/// Configuração do pipeline HTTP para ambiente não desenvolvimento.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

/// <summary>
/// Middleware base da aplicação.
/// </summary>
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

/// <summary>
/// Middleware de sessão.
/// </summary>
app.UseSession();

/// <summary>
/// Middleware de autenticação e autorização.
/// </summary>
app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Mapeamento das Razor Pages.
/// </summary>
app.MapRazorPages();

/// <summary>
/// Mapeamento de rotas MVC, caso existam controllers.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Aplicação de migrações e execução do seed inicial.
/// </summary>
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