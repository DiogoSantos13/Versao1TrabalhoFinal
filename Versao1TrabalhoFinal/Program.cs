using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Seed;
using Versao1TrabalhoFinal.Services;
using Versao1TrabalhoFinal.Services.AI;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração da base de dados da aplicação.
/// </summary>
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StandOficinaDB")));

/// <summary>
/// Configuração da cache em memória para suporte à sessão.
/// </summary>
builder.Services.AddDistributedMemoryCache();

/// <summary>
/// Configuração da sessão da aplicação.
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
/// Configuração do Identity com utilizadores e roles.
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
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
});

/// <summary>
/// Configuração das policies de autorização por perfis.
/// </summary>
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

/// <summary>
/// Configuração das Razor Pages e respetivas regras de acesso.
/// </summary>
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");

    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Index");
    options.Conventions.AllowAnonymousToPage("/VeiculosStand/Details");

    options.Conventions.AuthorizePage("/VeiculosStand/Create", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Edit", "StaffOnly");
    options.Conventions.AuthorizePage("/VeiculosStand/Delete", "StaffOnly");

    options.Conventions.AuthorizeFolder("/ClienteArea", "ClienteOnly");
    options.Conventions.AuthorizeFolder("/Dashboard", "AdminOnly");

    /// <summary>
    /// Autoriza o acesso à pasta de produtos a clientes e membros do staff definidos.
    /// </summary>
    options.Conventions.AuthorizeFolder("/Produtos", "ProdutosAccess");
});

/// <summary>
/// Registo dos serviços da aplicação.
/// </summary>
builder.Services.AddScoped<IOpenAIChatService, OpenAIChatService>();
builder.Services.AddScoped<IAiChatService, AiChatService>();
builder.Services.AddScoped<IChatConversationService, ChatConversationService>();
builder.Services.AddScoped<OrcamentoService>();

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
/// Middleware de sessão. Deve ser chamado após o routing.
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
/// Execução do seed inicial de roles e utilizadores.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();