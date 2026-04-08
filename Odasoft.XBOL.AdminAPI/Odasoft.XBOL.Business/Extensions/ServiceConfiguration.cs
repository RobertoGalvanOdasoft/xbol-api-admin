using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Extensions
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<AmenityService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ClientCreditAccountService>();
            services.AddScoped<ClientCreditTransactionService>();
            services.AddScoped<ClientService>();
            services.AddScoped<EventImageService>();
            services.AddScoped<EventScheduleService>();
            services.AddScoped<EventSeatsService>();
            services.AddScoped<EventSectionService>();
            services.AddScoped<EventService>();
            services.AddScoped<LegalRepresentativeService>();
            services.AddScoped<PhoneRegionCodesService>();
            services.AddScoped<OrderActionService>();
            services.AddScoped<ReportService>();
            services.AddScoped<OrderService>();
            services.AddScoped<SeatManagementService>();
            services.AddScoped<SeasonPassService>();
            services.AddScoped<SeasonSeatsService>();
            services.AddScoped<SeasonSectionService>();
            services.AddScoped<SeasonService>();
            services.AddScoped<SequenceTrackerService>();
            services.AddScoped<SuiteAgreementService>();
            services.AddScoped<SuiteLevelService>();
            services.AddScoped<SuiteService>();
            services.AddScoped<VenueAmenityService>();
            services.AddScoped<VenueImageService>();
            services.AddScoped<VenueMapService>();
            services.AddScoped<VenueService>();

            services.AddValidatorsFromAssembly(typeof(ServiceConfiguration).Assembly);

            return services;
        }
    }
}
