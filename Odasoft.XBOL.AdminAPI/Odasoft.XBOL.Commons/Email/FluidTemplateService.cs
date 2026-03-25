using System.Collections.Concurrent;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Odasoft.XBOL.Commons.Email;

public partial class FluidTemplateService(string templatePath, ILogger<FluidTemplateService> logger) : ITemplateService
{
    private readonly FluidParser _parser = new();
    private readonly PhysicalFileProvider _fileProvider = new(
        Directory.Exists(templatePath) ? templatePath : Directory.CreateDirectory(templatePath).FullName);
    private readonly TemplateOptions _options = CreateOptions();
    private readonly ConcurrentDictionary<string, IFluidTemplate> _templateCache = new();
    private readonly ConcurrentDictionary<Type, byte> _registeredTypes = new();

    public async Task<string> RenderAsync(string templateName, object model)
    {
        var template = GetOrParseTemplate(templateName);
        EnsureTypeRegistered(model.GetType());

        var context = new TemplateContext(model, _options);
        return await template.RenderAsync(context);
    }

    private IFluidTemplate GetOrParseTemplate(string templateName)
    {
        return _templateCache.GetOrAdd(templateName, name =>
        {
            var fileInfo = _fileProvider.GetFileInfo($"{name}.liquid");
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"Template '{name}.liquid' not found");

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
        if (_registeredTypes.TryAdd(type, 0))
        {
            lock (_options)
            {
                _options.MemberAccessStrategy.Register(type);
            }
        }
    }

    private static TemplateOptions CreateOptions()
    {
        var options = new TemplateOptions();
        options.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.CamelCase;
        options.MemberAccessStrategy.IgnoreCasing = true;
        return options;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Parsed and cached template '{TemplateName}'")]
    private static partial void LogTemplateParsed(ILogger logger, string templateName);
}
