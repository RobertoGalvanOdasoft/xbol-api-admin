using Microsoft.Extensions.DependencyInjection;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Client;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;

namespace Odasoft.XBOL.Data.Extensions
{
    public static class RepositoryConfiguration
    {
        public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<EventRepository>();
            services.AddScoped<EventSeatRepository>();
            services.AddScoped<EventScheduleRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<SuiteRepository>();
            services.AddScoped<SuiteLevelRepository>();
            services.AddScoped<VenueRepository>();
            services.AddScoped<SeasonPassRepository>();
            services.AddScoped<SeasonRepository>();
            services.AddScoped<ClientRepository>();

            return services;
        }
    }
}
