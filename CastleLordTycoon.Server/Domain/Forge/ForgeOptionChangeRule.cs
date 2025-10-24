using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeOptionChangeRule
{
    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("option_change_item_id")]
    public string OptionChangeItemId { get; init; } = string.Empty;

    [Name("runs_required_count")]
    public int RunsRequiredCount { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}