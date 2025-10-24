using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Npc;

public sealed class NpcService
{
    [Name("service_id")]
    public string ServiceId { get; init; } = string.Empty;

    [Name("service_name_ko")]
    public string ServiceNameKo { get; init; } = string.Empty;

    [Name("service_type")]
    public string ServiceType { get; init; } = string.Empty;

    [Name("description_ko")]
    public string DescriptionKo { get; init; } = string.Empty;
}
