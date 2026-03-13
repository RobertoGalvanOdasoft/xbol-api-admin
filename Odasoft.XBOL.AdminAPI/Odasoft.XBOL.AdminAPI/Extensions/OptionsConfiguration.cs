using Odasoft.XBOL.Commons.Options;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class OptionsConfiguration
{
    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TicketingClientOptions>()
            .BindConfiguration("TicketingClient")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<LocalizationOptions>()
            .BindConfiguration("Localization")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<IdentitySettingsOptions>()
            .BindConfiguration("Identity")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FileUploadOptions>()
            .BindConfiguration("FileUpload")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
