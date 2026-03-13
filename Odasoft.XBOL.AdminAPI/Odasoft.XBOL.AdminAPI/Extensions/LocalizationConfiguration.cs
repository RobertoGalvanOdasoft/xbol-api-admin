using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using LocalizationOptions = Odasoft.XBOL.Commons.Options.LocalizationOptions;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class LocalizationConfiguration
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddOptions<RequestLocalizationOptions>()
            .Configure<IOptions<LocalizationOptions>>((opts, loc) =>
            {
                opts.SetDefaultCulture(loc.Value.DefaultCulture);
                opts.AddSupportedCultures(loc.Value.SupportedCultures);
                opts.AddSupportedUICultures(loc.Value.SupportedCultures);
            });

        services.AddMvc()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var localizer = context.HttpContext.RequestServices
                    .GetRequiredService<IStringLocalizerFactory>()
                    .Create(typeof(SharedResource));

                var details = new ValidationProblemDetails(context.ModelState)
                {
                    Title = localizer["ValidationTitle"]
                };

                return new BadRequestObjectResult(details);
            };
        });

        return services;
    }
}
