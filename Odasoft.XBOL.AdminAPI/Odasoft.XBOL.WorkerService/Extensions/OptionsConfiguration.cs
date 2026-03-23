using Odasoft.XBOL.Commons.Options;

namespace Odasoft.XBOL.WorkerService.Extensions;

public static class OptionsConfiguration
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<BackgroundJobsOptions>()
            .BindConfiguration("BackgroundJobs")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
