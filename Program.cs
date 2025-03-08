using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Helpers;
using RunningApplicationNew.Hubs;
using RunningApplicationNew.IRepository;
using RunningApplicationNew.RepositoryLayer;
using RunningApplicationNew.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS Politikası Tanımla
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://*.replit.app", "https://*.repl.co")
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyOrigin() // Fallback for other origins
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Add services to the container.
builder.Services.AddControllers();

// Swagger ve OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Running Application API",
        Version = "v1",
        Description = "An API for the Running Application"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
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

// DbContext'i ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRaceRoomRepository, RaceRoomRepository>();
builder.Services.AddScoped<IUserResultsRepository, UserResultsRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourAppName",
            ValidAudience = "YourAppNameUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddSingleton<RaceUpdateService>();
builder.Services.AddHostedService<RaceUpdateService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Always enable Swagger on Replit
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Running Application API");
    // Make Swagger UI available at the application's root
    c.RoutePrefix = string.Empty;
});

// Add a redirect from the root to Swagger UI
app.MapGet("/", () => Results.Redirect("/index.html"));

app.UseHttpsRedirection();

// CORS Kullanımı Aktif Et!
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseWebSockets();
app.MapHub<RaceHub>("/racehub");

app.Run();