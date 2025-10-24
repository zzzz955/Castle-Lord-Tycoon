using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Gems;

public sealed class GemCombinationRule
{
    [Name("input_grade")]
    public string InputGrade { get; init; } = string.Empty;

    [Name("output_grade")]
    public string OutputGrade { get; init; } = string.Empty;

    [Name("success_rate_bps")]
    public int SuccessRateBasisPoints { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("required_shard_id")]
    public string RequiredShardId { get; init; } = string.Empty;

    [Name("required_shard_amount")]
    public int RequiredShardAmount { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
