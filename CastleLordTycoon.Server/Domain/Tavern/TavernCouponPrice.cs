using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Tavern;

public sealed class TavernCouponPrice
{
    [Name("tier_id")]
    public string TierId { get; init; } = string.Empty;

    [Name("coupon_item_id")]
    public string CouponItemId { get; init; } = string.Empty;

    [Name("coupon_name_ko")]
    public string CouponNameKo { get; init; } = string.Empty;

    [Name("gold_cost")]
    public int GoldCost { get; init; }

    [Name("drop_rate_bps")]
    public int DropRateBasisPoints { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
