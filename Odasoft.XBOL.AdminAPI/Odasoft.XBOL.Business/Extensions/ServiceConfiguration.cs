using Microsoft.Extensions.DependencyInjection;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Extensions
{
    public static class ServiceConfiguration
    {
        // Add your service configurations here in the future
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<EventService>();
            services.AddScoped<EventSeatsService>();
            services.AddScoped<EventSectionService>();
            services.AddScoped<OrderService>();
            services.AddScoped<SuiteLevelService>();
            services.AddScoped<SuiteService>();
            services.AddScoped<SuiteAgreementService>();
            services.AddScoped<VenueService>();
            services.AddScoped<SeasonService>();
            services.AddScoped<ClientService>();
            services.AddScoped<SeasonPassService>();

            return services;
        }
    }
}
