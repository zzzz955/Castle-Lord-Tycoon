using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Forge;

public sealed class ForgeDecomposeRule
{
    [Name("grade")]
    public string Grade { get; init; } = string.Empty;

    [Name("return_item_id")]
    public string ReturnItemId { get; init; } = string.Empty;

    [Name("return_amount_min")]
    public int ReturnAmountMin { get; init; }

    [Name("return_amount_max")]
    public int ReturnAmountMax { get; init; }

    [Name("return_currency")]
    public int ReturnCurrency { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
