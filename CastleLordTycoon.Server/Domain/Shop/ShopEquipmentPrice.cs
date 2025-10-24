using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Shop;

public sealed class ShopEquipmentPrice
{
    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("level_min")]
    public int LevelMin { get; init; }

    [Name("level_max")]
    public int LevelMax { get; init; }

    [Name("base_price_gold")]
    public int BasePriceGold { get; init; }

    [Name("price_formula")]
    public string PriceFormula { get; init; } = string.Empty;

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
