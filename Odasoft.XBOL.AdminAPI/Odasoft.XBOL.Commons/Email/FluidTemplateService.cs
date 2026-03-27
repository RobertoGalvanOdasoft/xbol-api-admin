using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace Odasoft.XBOL.Commons.Email;

public partial class FluidTemplateService(
    string templatePath,
    IStringLocalizer<EmailResource> localizer,
    ILogger<FluidTemplateService> logger) : ITemplateService
{
    private readonly FluidParser _parser = new();
    private readonly TemplateOptions _options = CreateOptions(templatePath);
    private readonly ConcurrentDictionary<string, IFluidTemplate> _templateCache = new();
    private readonly ConcurrentDictionary<Type, byte> _registeredTypes = new();

    public void ClearCache() => _templateCache.Clear();

    public async Task<string> RenderAsync(string templateName, object model)
    {
        var template = GetOrParseTemplate(templateName);
        EnsureTypeRegistered(model.GetType());

        var context = new TemplateContext(model, _options);
        context.AmbientValues[LocalizationFilters.LocalizerKey] = localizer;
        context.CultureInfo = System.Globalization.CultureInfo.CurrentCulture;
        return await template.RenderAsync(context);
    }

    private IFluidTemplate GetOrParseTemplate(string templateName)
    {
        return _templateCache.GetOrAdd(templateName, name =>
        {
            var fileInfo = _options.FileProvider.GetFileInfo($"{name}.liquid");
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Template '{name}.liquid' not found");
            }

            using var stream = fileInfo.CreateReadStream();
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            if (_parser.TryParse(content, out var parsed, out var error))
            {
                LogTemplateParsed(logger, name);
                return parsed;
            }

            throw new InvalidOperationException($"Failed to parse template '{name}': {error}");
        });
    }

    private void EnsureTypeRegistered(Type type)
    {
        if (!_registeredTypes.TryAdd(type, 0))
        {
            return;
        }

        lock (_options)
        {
            _options.MemberAccessStrategy.Register(type);
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (IsFluidNativeType(propertyType))
            {
                continue;
            }

            var enumerableType = propertyType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableType is not null)
            {
                var elementType = enumerableType.GetGenericArguments()[0];
                if (!IsFluidNativeType(elementType))
                {
                    EnsureTypeRegistered(elementType);
                }
            }
            else if (propertyType.IsClass || (propertyType.IsValueType && !propertyType.IsPrimitive))
            {
                EnsureTypeRegistered(propertyType);
            }
        }
    }

    private static bool IsFluidNativeType(Type type) =>
        type.IsPrimitive || type.IsEnum ||
        type == typeof(string) || type == typeof(decimal) ||
        type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
        type == typeof(Guid);

    private static TemplateOptions CreateOptions(string path)
    {
        var resolvedPath = Directory.Exists(path) ? path : Directory.CreateDirectory(path).FullName;
        var options = new TemplateOptions();
        options.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.CamelCase;
        options.MemberAccessStrategy.IgnoreCasing = true;
        options.FileProvider = new PhysicalFileProvider(resolvedPath);
        options.Filters.AddFilter("t", LocalizationFilters.Translate);
        options.Filters.AddFilter("t_format", LocalizationFilters.TranslateFormat);
        return options;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Parsed and cached template '{TemplateName}'")]
    private static partial void LogTemplateParsed(ILogger logger, string templateName);
}
