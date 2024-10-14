using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Storage.V1;
using ApiInmobiliariaAnNaTe.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5043", "http://*:5000", "https://*:5043");

// Inicializa Firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("C:\\Users\\Usuario\\ApiInmobiliariaAnNaTe\\utils\\appinmobiliaria-2d959-firebase-adminsdk-mamq8-e9365e2599.json"),
});

// Configuración de Google Cloud Storage
var credential = GoogleCredential.FromFile("C:\\Users\\Usuario\\ApiInmobiliariaAnNaTe\\utils\\appinmobiliaria-2d959-firebase-adminsdk-mamq8-e9365e2599.json");
builder.Services.AddSingleton(StorageClient.Create(credential));

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Configuración de JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor ingresa el token JWT en el formato 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
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
            new string[] {}
        }
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());


});



// Configuración de JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Cambiar a true en producción
    x.SaveToken = true;

    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// Configuración de controladores y base de datos
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 2, 12))));

builder.Services.AddScoped<Email>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Muestra detalles del error en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
        c.RoutePrefix = string.Empty; // Para que Swagger UI esté en la raíz
    });
}

// Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();


// Endpoints
app.MapControllers();
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
