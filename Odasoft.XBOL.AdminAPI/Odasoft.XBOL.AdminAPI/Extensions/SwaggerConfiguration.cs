using Microsoft.OpenApi;
using System.Reflection;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class SwaggerConfiguration
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "XBOL Admin API", Version = "v1" });

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.MapType<decimal>(() => new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal" });

            c.UseAllOfToExtendReferenceSchemas();
            c.SupportNonNullableReferenceTypes();
        }).AddSwaggerGenNewtonsoftSupport();

        return services;
    }
}
