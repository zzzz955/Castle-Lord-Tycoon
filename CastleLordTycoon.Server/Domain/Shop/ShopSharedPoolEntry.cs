using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopSharedPoolEntry
{
    [Name("item_id")]
    public string ItemId { get; init; } = string.Empty;

    [Name("item_name_ko")]
    public string ItemNameKo { get; init; } = string.Empty;

    [Name("item_type")]
    public string ItemType { get; init; } = string.Empty;

    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("weight_bps")]
    public int WeightBasisPoints { get; init; }

    [Name("base_price")]
    public int BasePrice { get; init; }

    [Name("currency")]
    public string Currency { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
