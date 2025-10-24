using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroDropTableEntry
{
    [Name("table_id")]
    public string TableId { get; init; } = string.Empty;

    [Name("hero_id")]
    public string HeroId { get; init; } = string.Empty;

    [Name("weight")]
    public int Weight { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
