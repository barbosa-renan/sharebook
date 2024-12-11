using Microsoft.OpenApi.Models;
using Sharebook.Infra;
using Sharebook.Infra.ServiceExtension;
using Sharebook.Services;
using Sharebook.Services.Interfaces;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis_cache:6379"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Product Pulse API",
        Description = "This powerful API, built on ASP.NET Core 6 with EF Core 6, provides seamless product management capabilities, including operations for inserting, deleting, updating, and retrieving products. Whether you're developing an e-commerce platform, inventory management system, or any application dealing with products, Sharebook API is the robust solution you need.",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Pulse API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz da aplicação
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
