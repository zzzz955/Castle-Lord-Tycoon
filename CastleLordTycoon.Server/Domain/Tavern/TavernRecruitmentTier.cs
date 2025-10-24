using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Tavern;

public sealed class TavernRecruitmentTier
{
    [Name("tier_id")]
    public string TierId { get; init; } = string.Empty;

    [Name("tier_name_ko")]
    public string TierNameKo { get; init; } = string.Empty;

    [Name("free_cooldown_hours")]
    public int FreeCooldownHours { get; init; }

    [Name("free_batch_size")]
    public int FreeBatchSize { get; init; }

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("coupon_item_id")]
    public string CouponItemId { get; init; } = string.Empty;

    [Name("required_hero_slot_free")]
    public int RequiredHeroSlotFree { get; init; }

    [Name("hero_star_min")]
    public int HeroStarMin { get; init; }

    [Name("hero_star_max")]
    public int HeroStarMax { get; init; }

    [Name("star1_bps")]
    public int Star1BasisPoints { get; init; }

    [Name("star2_bps")]
    public int Star2BasisPoints { get; init; }

    [Name("star3_bps")]
    public int Star3BasisPoints { get; init; }

    [Name("star4_bps")]
    public int Star4BasisPoints { get; init; }

    [Name("star5_bps")]
    public int Star5BasisPoints { get; init; }

    [Name("star6_bps")]
    public int Star6BasisPoints { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
