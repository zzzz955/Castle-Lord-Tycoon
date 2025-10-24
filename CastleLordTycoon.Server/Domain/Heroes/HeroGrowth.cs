using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroGrowth
{
    [Name("level")]
    public int Level { get; init; }

    [Name("exp_required")]
    public int ExpRequired { get; init; }

    [Name("cumulative_exp")]
    public int CumulativeExp { get; init; }

    [Name("max_growth_cap")]
    public int MaxGrowthCap { get; init; }

    [Name("rebirth_eligible")]
    public bool RebirthEligible { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
