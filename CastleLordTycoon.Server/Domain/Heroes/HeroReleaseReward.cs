using CsvHelper.Configuration.Attributes;

namespace CastleLordTycoon.Server.Domain.Heroes;

public sealed class HeroReleaseReward
{
    [Name("star_rating")]
    public int StarRating { get; init; }

    [Name("fragment_item_id")]
    public string FragmentItemId { get; init; } = string.Empty;

    [Name("fragment_amount")]
    public int FragmentAmount { get; init; }

    [Name("notes_ko")]
    public string NotesKo { get; init; } = string.Empty;
}
