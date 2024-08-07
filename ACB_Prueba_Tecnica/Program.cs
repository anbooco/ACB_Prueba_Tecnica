using ACB_Prueba_Tecnica.DAL.Context;
using ACB_Prueba_Tecnica.DAL.Repositories;
using ACB_Prueba_Tecnica.Domain.Interfaces.Repositories;
using ACB_Prueba_Tecnica.Domain.Interfaces.Services;
using ACB_Prueba_Tecnica.Services;
using ACB_Prueba_Tecnica.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddAuthentication("TokenAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("TokenAuthentication", null);
builder.Services.AddAuthentication();

//Configuracion CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ACBContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v0",
        Title = "ACB Prueba Técnica",
        Description = "API para la Prueba Técnica de ACB"
    });

    //TODO: Only Debbug
    //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "API para la Prueba Técnica de ACB",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });

    // INFO - XML File - S0ha d'afegir al .csproj l'opcio <GenerateDocumentationFile>true</GenerateDocumentationFile>
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddHttpClient();

//Services
builder.Services.AddScoped<IACBService, ACBService>();

//Repositories
builder.Services.AddScoped<IPlayByPlayLeanRepository, PlayByPlayLeanRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
