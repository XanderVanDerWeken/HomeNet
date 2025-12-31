namespace HomeNet.Web.Configurations;

public sealed record CacheInitializerConfiguration
{
    public required string SchemaFileName { get; init; }
}
