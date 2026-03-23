using Hangfire;
using Odasoft.XBOL.Commons.BackgroundJobs;
using Odasoft.XBOL.Commons.Options;

namespace Odasoft.XBOL.WorkerService.Extensions;

public static class BackgroundJobsConfiguration
{
    public static IServiceCollection ConfigureBackgroundJobs(
        this IServiceCollection services, IConfiguration configuration)
    {
        var options = new BackgroundJobsOptions();
        configuration.GetRequiredSection("BackgroundJobs").Bind(options);

        services.AddHangfire(config => config.UseDefaultStorage(options.ConnectionString));

        services.AddHangfireServer(opts =>
        {
            opts.WorkerCount = options.WorkerCount > 0
                ? options.WorkerCount
                : Environment.ProcessorCount * 2;
            opts.Queues = ["default"];
            opts.ServerTimeout = TimeSpan.FromMinutes(options.ServerTimeoutMinutes);
            opts.ShutdownTimeout = TimeSpan.FromSeconds(options.ShutdownTimeoutSeconds);
        });

        return services;
    }
}
