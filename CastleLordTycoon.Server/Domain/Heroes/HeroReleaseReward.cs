using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroReleaseReward
{
    [Name("star_rating")]
    public int StarRating { get; init; }

    [Name("reward_item_id")]
    public string RewardItemId { get; init; } = string.Empty;

    [Name("reward_amount")]
    public int RewardAmount { get; init; }

    [Name("reward_currency")]
    public int RewardCurrency { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
