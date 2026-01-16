using Microsoft.Extensions.DependencyInjection;
using Odasoft.XBOL.Data.Repositories;

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

            return services;
        }
    }
}
