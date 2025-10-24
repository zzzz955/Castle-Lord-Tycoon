using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Gems;

public sealed class GemDropTableEntry
{
    [Name("drop_id")]
    public string DropId { get; init; } = string.Empty;

    [Name("source_type")]
    public string SourceType { get; init; } = string.Empty;

    [Name("source_ref")]
    public string SourceRef { get; init; } = string.Empty;

    [Name("gem_id")]
    public string GemId { get; init; } = string.Empty;

    [Name("weight_bps")]
    public int WeightBasisPoints { get; init; }

    [Name("amount_min")]
    public int AmountMin { get; init; }

    [Name("amount_max")]
    public int AmountMax { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
