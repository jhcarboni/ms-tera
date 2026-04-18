using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MsTera.Api.Endpoints;
using MsTera.Shared.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MsTera API", Version = "v1" });
    
    // Configurar o botão "Authorize" no Swagger para usarmos nosso JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Cole apenas o token JWT (sem a palavra Bearer)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// Configurar o gerador de JWT
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Configurar Autenticação
builder.Services.AddAuthentication(options =>
{
    // O esquema padrão para proteger nossas rotas será o JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
    // Precisamos de um esquema padrão de SignIn para o Google salvar o cookie temporário
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // Adiciona o suporte a cookies que o GoogleDefaults precisa
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is missing"))),
        ValidateIssuer = true,
        ValidIssuer = "MsTera",
        ValidateAudience = true,
        ValidAudience = "MsTera",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is missing");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is missing");
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsTera API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Registrar endpoints
app.MapAuthEndpoints();

app.Run();
