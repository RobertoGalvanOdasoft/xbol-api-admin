using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Options;
using Odasoft.XBOL.WorkerService.Jobs;

namespace Odasoft.XBOL.WorkerService.Extensions;

public static class EmailConfiguration
{
    public static IServiceCollection ConfigureEmail(this IServiceCollection services)
    {
        services.AddOptions<SmtpOptions>()
            .BindConfiguration("Smtp")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IEmailService, SmtpEmailService>();

        services.AddSingleton<ITemplateService>(sp =>
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
            var logger = sp.GetRequiredService<ILogger<FluidTemplateService>>();
            return new FluidTemplateService(templatePath, logger);
        });

        services.AddScoped<IEmailJob, EmailJob>();

        return services;
    }
}
