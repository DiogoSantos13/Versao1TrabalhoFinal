using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Seed;
using Versao1TrabalhoFinal.Services;
using Versao1TrabalhoFinal.Services.AI;

var builder = WebApplication.CreateBuilder(args);

// Base de dados
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StandOficinaDB")));

// Sessão e cache
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Identity
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

// Cookie de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
});

// Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClienteOnly", policy =>
        policy.RequireRole("Cliente"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Mecanico", "Colaborador", "Vendedor", "Rececionista"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// Razor Pages
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
});


// Serviços da aplicação
builder.Services.AddScoped<IOpenAIChatService, OpenAIChatService>();
builder.Services.AddScoped<IAiChatService, AiChatService>();
builder.Services.AddScoped<IChatConversationService, ChatConversationService>();
builder.Services.AddScoped<OrcamentoService>();

var app = builder.Build();

// Pipeline
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


// Seed inicial de roles/users
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
