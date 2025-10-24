using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Territory;

public sealed class ForbiddenTile
{
    [Name("terrain_id")]
    public string TerrainId { get; init; } = string.Empty;

    [Name("terrain_name_ko")]
    public string TerrainNameKo { get; init; } = string.Empty;

    [Name("reason_ko")]
    public string ReasonKo { get; init; } = string.Empty;
}
