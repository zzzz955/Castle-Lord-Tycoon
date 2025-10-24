using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeSynthesisRule
{
    [Name("input_grade")]
    public string InputGrade { get; init; } = string.Empty;

    [Name("result_grade")]
    public string ResultGrade { get; init; } = string.Empty;

    [Name("input_required_count")]
    public int InputRequiredCount { get; init; }

    [Name("catalyst_item_id")]
    public string CatalystItemId { get; init; } = string.Empty;

    [Name("catalyst_amount")]
    public int CatalystAmount { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("success_rate_bps")]
    public int SuccessRateBasisPoints { get; init; }

    [Name("protect_on_fail")]
    public bool ProtectOnFail { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
