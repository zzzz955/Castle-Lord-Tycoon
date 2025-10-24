using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Npc;

public sealed class SettlementNpcMapEntry
{
    [Name("settlement_id")]
    public string SettlementId { get; init; } = string.Empty;

    [Name("service_id")]
    public string ServiceId { get; init; } = string.Empty;

    [Name("npc_name_ko")]
    public string NpcNameKo { get; init; } = string.Empty;

    [Name("position_tag")]
    public string PositionTag { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
