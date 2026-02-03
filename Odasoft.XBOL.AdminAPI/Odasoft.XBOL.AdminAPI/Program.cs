using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.Data.Extensions;
using Odasoft.XBOL.Models;
using System.Reflection;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<XBOLDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity + EF Core store
builder.Services.AddDataProtection();

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<XBOLDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
}).AddNewtonsoftJson();

// Add health check services
builder.Services.AddHealthChecks();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "XBOL Admin API", Version = "v1" });

    // Include XML comments if available
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.MapType<decimal>(() => new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal" });
}).AddSwaggerGenNewtonsoftSupport();

builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(CreateBookingCommand).Assembly);
});

// Add Http Clients
builder.Services.AddHttpClient<ITicketingClient, TicketingClient>(
    (provider, client) =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue("TicketingClientBaseAddress", "https://localhost:7021/"));
    });

var app = builder.Build();

// Enable middleware to serve generated OpenAPI as a JSON endpoint and the Swagger UI.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/admin-api.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/admin-api.json", "Admin API");
    });

    app.MapGet(
        "/",
        context =>
        {
            context.Response.Redirect("/swagger/index.html");
            return Task.CompletedTask;
        }
    );
}

// Configure the HTTP request pipeline.

// Only use HTTPS redirection when running directly (Visual Studio, dotnet run)
// Containers handle TLS at load balancer/reverse proxy level
if (!app.Environment.IsProduction()
    || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map health check endpoint for container health monitoring
app.MapHealthChecks("/healthz");
app.Run();
