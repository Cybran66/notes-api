using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotesApi.Data;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notes API (RU)",
        Version = "v1",
        Description = "Портфолио backend: заметки, поиск и история изменений."
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "Документация Notes API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API v1 (RU)");
    options.InjectStylesheet("/swagger-ui/custom.css?v=20260402-2");
    options.InjectJavascript("/swagger-ui/custom.js?v=20260402-2");
    options.DefaultModelsExpandDepth(-1);
    options.DisplayRequestDuration();
    options.EnableTryItOutByDefault();
    options.EnableFilter();
    options.DocExpansion(DocExpansion.List);
});

app.UseAuthorization();
app.MapControllers();

await SeedData.InitializeAsync(app.Services);

app.Run();
