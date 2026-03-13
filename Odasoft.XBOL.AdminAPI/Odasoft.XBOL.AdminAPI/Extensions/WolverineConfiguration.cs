using Odasoft.XBOL.Business.Messages;
using Wolverine;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class WolverineConfiguration
{
    public static IHostBuilder ConfigureWolverine(this IHostBuilder host)
    {
        host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(CreateEventBookingCommand).Assembly);
        });

        return host;
    }
}
