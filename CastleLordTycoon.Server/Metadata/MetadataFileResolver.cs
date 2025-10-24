using CastleLordTycoon.Server.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CastleLordTycoon.Server.Metadata;

public interface IMetadataFileResolver
{
    string Resolve(string fileName);
}

/// <summary>
/// Resolves metadata file paths and verifies their existence.
/// </summary>
public sealed class MetadataFileResolver : IMetadataFileResolver
{
    private readonly string _rootPath;
    private readonly ILogger<MetadataFileResolver> _logger;

    public MetadataFileResolver(
        IOptions<MetadataOptions> options,
        IHostEnvironment environment,
        ILogger<MetadataFileResolver> logger)
    {
        _logger = logger;
        var configuredPath = options.Value.DataRootPath;
        _rootPath = Path.GetFullPath(configuredPath, environment.ContentRootPath);
        _logger.LogInformation("Metadata root resolved to {RootPath}", _rootPath);
    }

    public string Resolve(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name must not be empty.", nameof(fileName));
        }

        var fullPath = Path.Combine(_rootPath, fileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Metadata file not found: {fullPath}", fullPath);
        }

        return fullPath;
    }
}
