using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Odasoft.XBOL.Commons.Options;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.AdminAPI.Extensions;

public static class IdentityConfiguration
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddDataProtection();

        services.AddOptions<IdentityOptions>()
            .Configure<IOptions<IdentitySettingsOptions>>((identity, settings) =>
            {
                identity.Password.RequireDigit = settings.Value.Password.RequireDigit;
                identity.Password.RequiredLength = settings.Value.Password.RequiredLength;
                identity.User.RequireUniqueEmail = settings.Value.User.RequireUniqueEmail;
            });

        services.AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<XBOLDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}
