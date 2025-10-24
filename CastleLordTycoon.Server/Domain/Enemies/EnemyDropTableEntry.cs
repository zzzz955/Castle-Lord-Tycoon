using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Enemies;

public sealed class EnemyDropTableEntry
{
    [Name("drop_table_id")]
    public string DropTableId { get; init; } = string.Empty;

    [Name("entry_id")]
    public string EntryId { get; init; } = string.Empty;

    [Name("reward_type")]
    public string RewardType { get; init; } = string.Empty;

    [Name("reward_id")]
    public string RewardId { get; init; } = string.Empty;

    [Name("reward_name_ko")]
    public string RewardNameKo { get; init; } = string.Empty;

    [Name("drop_rate_bps")]
    public int DropRateBasisPoints { get; init; }

    [Name("amount_min")]
    public int AmountMin { get; init; }

    [Name("amount_max")]
    public int AmountMax { get; init; }

    [Name("value_unit")]
    public string ValueUnit { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
