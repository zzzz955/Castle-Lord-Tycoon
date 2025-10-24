using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroRebirthRule
{
    [Name("material_level_min")]
    public int MaterialLevelMin { get; init; }

    [Name("material_level_max")]
    public int MaterialLevelMax { get; init; }

    [Name("growth_transfer_pct")]
    public int GrowthTransferPct { get; init; }

    [Name("success_rate_pct")]
    public int SuccessRatePct { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
