using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Options;
using Odasoft.XBOL.WorkerService.Jobs;

namespace Odasoft.XBOL.WorkerService.Extensions;

public static class EmailConfiguration
{
    public static IServiceCollection ConfigureEmail(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddOptions<SmtpOptions>()
            .BindConfiguration("Smtp")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IEmailService, SmtpEmailService>();

        services.AddSingleton<ITemplateService>(sp =>
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
            var localizer = sp.GetRequiredService<IStringLocalizer<EmailResource>>();
            var logger = sp.GetRequiredService<ILogger<FluidTemplateService>>();
            return new FluidTemplateService(templatePath, localizer, logger);
        });

        services.AddScoped<IEmailJob, EmailJob>();

        return services;
    }
}
