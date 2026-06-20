using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection; // <<-- Adicionado para expor métodos de extensão de health checks (AddDbContextCheck)
using Versao1TrabalhoFinal.Api.Middleware;
using Versao1TrabalhoFinal.Api.Models;
using Versao1TrabalhoFinal.Api.Services;
using Versao1TrabalhoFinal.Data;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Carrega as definições JWT a partir da configuração.
/// </summary>
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

/// <summary>
/// Obtém a connection string principal da aplicação.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi encontrada.");

/// <summary>
/// Regista o contexto da base de dados com SQL Server.
/// </summary>
builder.Services.AddDbContext<StandDbContext>(options =>
    options.UseSqlServer(connectionString));

/// <summary>
/// Regista o ASP.NET Core Identity com utilizadores e roles.
/// </summary>
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<StandDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Lê as definições JWT da configuração.
/// </summary>
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("A secção 'Jwt' não foi encontrada.");

/// <summary>
/// Converte a chave JWT para bytes.
/// </summary>
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

/// <summary>
/// Configura a autenticação JWT Bearer.
/// </summary>
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

/// <summary>
/// Regista o serviço de autorização.
/// </summary>
builder.Services.AddAuthorization();

/// <summary>
/// Regista o serviço responsável pela geração de tokens JWT.
/// </summary>
builder.Services.AddScoped<JwtService>();

/// <summary>
/// Regista os controllers da API.
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Regista ProblemDetails para respostas de erro padronizadas.
/// </summary>
builder.Services.AddProblemDetails();

/// <summary>
/// Configura a política CORS para permitir chamadas do frontend.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

/// <summary>
/// Regista health checks, incluindo verificação do contexto da base de dados.
/// </summary>
builder.Services.AddHealthChecks();
    

/// <summary>
/// Configura rate limiting global da API.
/// </summary>
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    /// <summary>
    /// Política geral para a maioria dos endpoints.
    /// </summary>
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    /// <summary>
    /// Política mais restrita para endpoints de autenticação.
    /// </summary>
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    /// <summary>
    /// Define a resposta devolvida quando o limite é excedido.
    /// </summary>
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync(
            """
            {
              "title": "Demasiados pedidos",
              "status": 429,
              "detail": "Foi excedido o limite de pedidos. Tente novamente dentro de instantes."
            }
            """,
            cancellationToken);
    };
});

/// <summary>
/// Regista os serviços necessários para o Swagger/OpenAPI.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Versao1TrabalhoFinal API",
        Version = "v1",
        Description = "API do projeto Versao1TrabalhoFinal com autenticação JWT."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduza o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

/// <summary>
/// Ativa o middleware global de tratamento de exceções.
/// </summary>
app.UseMiddleware<ExceptionMiddleware>();

/// <summary>
/// Ativa o middleware de logging de pedidos.
/// </summary>
app.UseRequestLogging();

/// <summary>
/// Ativa o Swagger em ambiente de desenvolvimento.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Redireciona pedidos HTTP para HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Ativa a política CORS configurada.
/// </summary>
app.UseCors("FrontendPolicy");

/// <summary>
/// Ativa o middleware de rate limiting.
/// </summary>
app.UseRateLimiter();

/// <summary>
/// Ativa o middleware de autenticação.
/// </summary>
app.UseAuthentication();

/// <summary>
/// Ativa o middleware de autorização.
/// </summary>
app.UseAuthorization();

/// <summary>
/// Mapeia os endpoints dos controllers com a política geral de rate limiting.
/// </summary>
app.MapControllers()
   .RequireRateLimiting("fixed");

/// <summary>
/// Mapeia o endpoint de health check.
/// </summary>
app.MapHealthChecks("/health");

/// <summary>
/// Executa migrations pendentes e cria roles/utilizador admin no arranque.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<StandDbContext>();

    await dbContext.Database.MigrateAsync();
    await IdentitySeeder.SeedRolesAndAdminAsync(services);
}

/// <summary>
/// Inicia a aplicação.
/// </summary>
app.Run();