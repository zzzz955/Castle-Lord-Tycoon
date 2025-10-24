using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeOptionChangeRule
{
    [Name("grade_min")]
    public string GradeMin { get; init; } = string.Empty;

    [Name("grade_max")]
    public string GradeMax { get; init; } = string.Empty;

    [Name("option_change_item_id")]
    public string OptionChangeItemId { get; init; } = string.Empty;

    [Name("item_cost")]
    public int ItemCost { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("reroll_stat_count")]
    public int RerollStatCount { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
