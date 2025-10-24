using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroPoolExpansionRule
{
    [Name("current_slot_count")]
    public int CurrentSlotCount { get; init; }

    [Name("expansion_cost_gold")]
    public int ExpansionCostGold { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
