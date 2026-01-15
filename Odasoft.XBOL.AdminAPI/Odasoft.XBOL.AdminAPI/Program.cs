using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.AdminAPI;
using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.Data.Extensions;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<XBOLDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity + EF Core store
builder.Services.AddDataProtection();

// Add services to the container.
builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();

builder.Services.AddControllers();

// Add health check services
builder.Services.AddHealthChecks();

// Add OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseWolverine();

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
    app.UseSwagger();
    app.UseSwaggerUI();

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
if (!app.Environment.IsProduction() ||
    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

// Map health check endpoint for container health monitoring
app.MapHealthChecks("/healthz");

app.Run();
