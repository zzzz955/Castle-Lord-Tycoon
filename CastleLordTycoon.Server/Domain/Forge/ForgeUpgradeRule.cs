using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeUpgradeRule
{
    [Name("upgrade_min")]
    public int UpgradeMin { get; init; }

    [Name("upgrade_max")]
    public int UpgradeMax { get; init; }

    [Name("success_rate_bps")]
    public int SuccessRateBasisPoints { get; init; }

    [Name("cost_multiplier_pct")]
    public int CostMultiplierPct { get; init; }

    [Name("fail_penalty_level")]
    public int FailPenaltyLevel { get; init; }

    [Name("protect_scroll_applicable")]
    public bool ProtectScrollApplicable { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
