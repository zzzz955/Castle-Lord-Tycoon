using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopSharedPrice
{
    [Name("item_id")]
    public string ItemId { get; init; } = string.Empty;

    [Name("base_price_gold")]
    public int BasePriceGold { get; init; }

    [Name("bulk_limit")]
    public int BulkLimit { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
