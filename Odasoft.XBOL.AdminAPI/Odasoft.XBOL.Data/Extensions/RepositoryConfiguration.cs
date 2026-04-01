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
            services.AddScoped<ClientRepository>();

            services.AddScoped<OrderRepository>();

            services.AddScoped<SeasonRepository>();
            services.AddScoped<SeasonSeatRepository>();
            services.AddScoped<SeasonSectionRepository>();

            services.AddScoped<SeasonPassRepository>();

            services.AddScoped<ClientCreditAccountRepository>();
            services.AddScoped<ClientCreditTransactionRepository>();
            services.AddScoped<EventRepository>();
            services.AddScoped<EventImageRepository>();
            services.AddScoped<EventScheduleRepository>();
            services.AddScoped<EventSeatRepository>();
            services.AddScoped<EventSectionRepository>();
            services.AddScoped<LegalRepresentativeRepository>();
            services.AddScoped<OrderActionLogRepository>();
            services.AddScoped<PhoneRegionCodeRepository>();
            services.AddScoped<RoleRepository>();
            services.AddScoped<SequenceTrackerRepository>();
            services.AddScoped<SuiteAgreementFileRepository>();
            services.AddScoped<SuiteAgreementRepository>();
            services.AddScoped<SuiteLevelRepository>();
            services.AddScoped<SuiteRepository>();
            services.AddScoped<TicketRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<VenueImageRepository>();
            services.AddScoped<VenueMapRepository>();
            services.AddScoped<VenueRepository>();

            return services;
        }
    }
}
