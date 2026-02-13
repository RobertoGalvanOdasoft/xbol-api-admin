using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace Odasoft.XBOL.DTO.Helpers
{
    public sealed class DateGreaterThanAttribute(string comparisonProperty) : ValidationAttribute
    {
        public string ComparisonProperty { get; } = comparisonProperty;

        /// <summary>
        /// Validates that a DateTimeOffset property is greater than another DateTimeOffset property.
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = ToNullableDateTimeOffset(value);

            if (currentValue is null)
                return ValidationResult.Success;

            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(ComparisonProperty)
                ?? throw new ArgumentException($"Property '{ComparisonProperty}' not found on '{validationContext.ObjectType.Name}'.");

            var rawComparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);
            var comparisonValue = ToNullableDateTimeOffset(rawComparisonValue);

            if (comparisonValue is null)
                return ValidationResult.Success;

            if (currentValue <= comparisonValue)
                return new ValidationResult(GetLocalizedErrorMessage(validationContext, comparisonPropertyInfo));

            return ValidationResult.Success;
        }

        private static DateTimeOffset? ToNullableDateTimeOffset(object? value)
        {
            if (value is DateTimeOffset dateValue && dateValue != DateTimeOffset.MinValue)
            {
                return dateValue;
            }

            return null;
        }

        private string GetLocalizedErrorMessage(ValidationContext validationContext, PropertyInfo comparisonPropertyInfo)
        {
            var resourceKey = ErrorMessage ?? nameof(DateGreaterThanAttribute).Replace("Attribute", "");
            var displayName = validationContext.DisplayName;

            if (validationContext.GetService(typeof(IStringLocalizerFactory)) is not IStringLocalizerFactory factory)
                return string.Format(resourceKey, displayName, ComparisonProperty);

            var entryAssembly = Assembly.GetEntryAssembly()!;
            var localizer = factory.Create("SharedResource", entryAssembly.GetName().Name!);

            var template = localizer[resourceKey];
            var comparisonFieldName = localizer[GetDisplayName(comparisonPropertyInfo)];

            return string.Format(template, displayName, comparisonFieldName);
        }

        private static string GetDisplayName(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            return display?.GetName() ?? property.Name;
        }
    }
}
