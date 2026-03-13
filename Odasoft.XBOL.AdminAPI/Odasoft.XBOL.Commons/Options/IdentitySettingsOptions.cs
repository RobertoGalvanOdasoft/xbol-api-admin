using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.Commons.Options;

public class IdentitySettingsOptions
{
    [Description("Password policy settings")]
    public PasswordOptions Password { get; set; } = new();

    [Description("User account settings")]
    public UserOptions User { get; set; } = new();

    public class PasswordOptions
    {
        [DefaultValue(true)]
        [Description("Whether passwords must contain at least one digit")]
        public bool RequireDigit { get; set; } = true;

        [Range(1, 128)]
        [DefaultValue(8)]
        [Description("Minimum required password length")]
        public int RequiredLength { get; set; } = 8;
    }

    public class UserOptions
    {
        [DefaultValue(true)]
        [Description("Whether each user must have a unique email address")]
        public bool RequireUniqueEmail { get; set; } = true;
    }
}
