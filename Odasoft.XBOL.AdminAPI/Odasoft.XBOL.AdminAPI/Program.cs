using Hangfire;
using Microsoft.Extensions.Options;
using Odasoft.XBOL.AdminAPI.Extensions;
using Odasoft.XBOL.AdminAPI.Schema;
using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Data.Extensions;
using System.Globalization;
using LocalizationOptions = Odasoft.XBOL.Commons.Options.LocalizationOptions;

if (args.Contains("--generate-schema"))
{
    var outputPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "appsettings.schema.json"));
    AppSettingsSchemaGenerator.GenerateAndWrite(outputPath);
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureBackgroundJobs(builder.Configuration);

// Security
builder.Services.ConfigureIdentity();

// Application
builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();
builder.Host.ConfigureWolverine();

// Web framework
builder.Services.ConfigureMvc();
builder.Services.AddHealthChecks();
builder.Services.ConfigureLocalization();
builder.Services.ConfigureSwagger();

// External clients
builder.Services.ConfigureHttpClients();

var app = builder.Build();

// Enable middleware to serve generated OpenAPI as a JSON endpoint and the Swagger UI.
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");

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

// Only use HTTPS redirection when running directly (Visual Studio, dotnet run)
// Containers handle TLS at load balancer/reverse proxy level
if (!app.Environment.IsProduction()
    || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

var defaultCulture = new CultureInfo(
    app.Services.GetRequiredService<IOptions<LocalizationOptions>>().Value.DefaultCulture);
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

app.UseRequestLocalization();

app.MapControllers();

app.MapHealthChecks("/healthz");
app.Run();
