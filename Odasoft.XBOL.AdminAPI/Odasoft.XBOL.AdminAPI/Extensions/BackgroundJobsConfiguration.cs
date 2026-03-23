using Hangfire;
using Odasoft.XBOL.Commons.BackgroundJobs;
using Odasoft.XBOL.Commons.Options;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class BackgroundJobsConfiguration
{
    public static IServiceCollection ConfigureBackgroundJobs(
        this IServiceCollection services, IConfiguration configuration)
    {
        var options = new BackgroundJobsOptions();
        configuration.GetRequiredSection("BackgroundJobs").Bind(options);

        services.AddHangfire(config => config.UseDefaultStorage(options.ConnectionString));

        return services;
    }
}
