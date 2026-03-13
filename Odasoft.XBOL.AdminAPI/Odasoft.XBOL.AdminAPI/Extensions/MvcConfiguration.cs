using Microsoft.Extensions.Options;
using Odasoft.XBOL.AdminAPI.Filters;
using Odasoft.XBOL.Commons.Options;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class MvcConfiguration
{
    public static IServiceCollection ConfigureMvc(this IServiceCollection services)
    {
        services.AddOptions<Microsoft.AspNetCore.Http.Features.FormOptions>()
            .Configure<IOptions<FileUploadOptions>>((form, fileUpload) =>
            {
                form.MultipartBodyLengthLimit = fileUpload.Value.MultipartBodyLengthLimit;
            });

        services.AddControllers(options =>
        {
            options.Filters.Add(new ValidationFilter());
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        });

        return services;
    }
}
