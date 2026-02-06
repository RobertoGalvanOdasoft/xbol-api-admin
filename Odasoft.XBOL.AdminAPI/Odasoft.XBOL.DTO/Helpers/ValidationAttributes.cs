using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.DTO.Helpers
{
    public sealed class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        /// <summary>
        /// Validates that a DateTimeOffset property is greater than another DateTimeOffset property.
        /// </summary>
        /// <param name="comparisonProperty"></param>
        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle nullability
            }

            var currentValue = (DateTimeOffset)value;
            var comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonPropertyInfo == null)
            {
                throw new ArgumentException("Comparison property not found.");
            }

            var comparisonValue = (DateTimeOffset)comparisonPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ??
                    $"{validationContext.DisplayName} must be after {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
