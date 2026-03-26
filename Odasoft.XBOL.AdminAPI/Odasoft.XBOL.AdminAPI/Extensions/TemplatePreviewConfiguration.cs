using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Commons.Email;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class TemplatePreviewConfiguration
{
    public static IServiceCollection ConfigureTemplatePreview(
        this IServiceCollection services, IWebHostEnvironment environment)
    {
        var templatePath = Path.GetFullPath(
            Path.Combine(environment.ContentRootPath, "..", "Odasoft.XBOL.WorkerService", "Templates"));

        services.AddSingleton<ITemplateService>(sp =>
        {
            var localizer = sp.GetRequiredService<IStringLocalizer<EmailResource>>();
            var logger = sp.GetRequiredService<ILogger<FluidTemplateService>>();
            return new FluidTemplateService(templatePath, localizer, logger);
        });

        return services;
    }
}
