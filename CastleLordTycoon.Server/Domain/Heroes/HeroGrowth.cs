using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroGrowth
{
    [Name("level")]
    public int Level { get; init; }

    [Name("exp_required")]
    public int ExpRequired { get; init; }

    [Name("total_growth_modifier")]
    public double TotalGrowthModifier { get; init; }
}
